using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.MockEngine.Processing;
using MockHub.MockEngine.Routing;
using MockHub.MockEngine.Templates;

namespace MockHub.MockEngine.Middleware;

public class MockRoutingMiddleware<TDbContext> where TDbContext : DbContext
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MockRoutingMiddleware<TDbContext>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TemplateEngine _templateEngine;
    private readonly RouteMatcher _routeMatcher;

    // Reserved paths that should not be handled by mock routing
    private static readonly string[] ReservedPaths = new[]
    {
        "/_blazor",
        "/_framework",
        "/api",
        "/account",
        "/projects",
        "/teams",
        "/servers",
        "/logs",
        "/variables",
        "/settings",
        "/import-export",
        "/admin",
        "/setup",
        "/favicon",
        "/css",
        "/js",
        "/lib",
        "/mockhub"
    };

    public MockRoutingMiddleware(
        RequestDelegate next,
        ILogger<MockRoutingMiddleware<TDbContext>> logger,
        IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _templateEngine = new TemplateEngine();
        _routeMatcher = new RouteMatcher();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";

        // Skip root path
        if (path == "/")
        {
            await _next(context);
            return;
        }

        // Skip reserved paths
        if (IsReservedPath(path))
        {
            await _next(context);
            return;
        }

        // Skip static files
        if (path.Contains('.') && !path.EndsWith(".json") && !path.EndsWith(".xml"))
        {
            await _next(context);
            return;
        }

        // Try to match mock endpoint
        var mockResult = await TryHandleMockRequest(context, path);
        
        if (!mockResult)
        {
            // No mock found, continue to next middleware
            await _next(context);
        }
    }

    private bool IsReservedPath(string path)
    {
        return ReservedPaths.Any(reserved => 
            path.Equals(reserved, StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(reserved + "/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(reserved + "?", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<bool> TryHandleMockRequest(HttpContext context, string path)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Parse path to get project/team slugs
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (pathSegments.Length < 1)
        {
            return false;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var notifier = scope.ServiceProvider.GetService<IRequestLogNotifier>();

        var teams = dbContext.Set<Team>();
        var projects = dbContext.Set<MockProject>();
        var requestLogs = dbContext.Set<RequestLog>();

        // Try to find project
        MockProject? project = null;
        string endpointPath;

        // Check if first segment is a team slug
        var firstSegment = pathSegments[0];
        var team = await teams.FirstOrDefaultAsync(t => t.Slug == firstSegment && t.IsActive);

        if (team != null && pathSegments.Length >= 2)
        {
            // Team project: /{team_slug}/{project_slug}/...
            var projectSlug = pathSegments[1];
            project = await projects
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.Responses)
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.ValidationRules)
                .FirstOrDefaultAsync(p => p.TeamId == team.Id && p.Slug == projectSlug && p.IsActive);

            endpointPath = pathSegments.Length > 2 
                ? "/" + string.Join("/", pathSegments.Skip(2)) 
                : "/";
        }
        else
        {
            // Personal project: /{project_slug}/...
            project = await projects
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.Responses)
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.ValidationRules)
                .FirstOrDefaultAsync(p => p.TeamId == null && p.Slug == firstSegment && p.IsActive);

            if (project == null)
            {
                return false;
            }

            endpointPath = pathSegments.Length > 1 
                ? "/" + string.Join("/", pathSegments.Skip(1)) 
                : "/";
        }

        if (project == null)
        {
            return false;
        }

        // Handle OPTIONS request for CORS
        if (context.Request.Method == "OPTIONS" && project.EnableCors)
        {
            context.Response.StatusCode = 204;
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD";
            context.Response.Headers["Access-Control-Allow-Headers"] = "*";
            context.Response.Headers["Access-Control-Max-Age"] = "86400";
            return true;
        }

        // Find matching endpoint
        var httpMethod = context.Request.Method;
        var matchingEndpoint = FindMatchingEndpoint(project.Endpoints, httpMethod, endpointPath, out var routeParams);

        if (matchingEndpoint == null)
        {
            _logger.LogDebug("No endpoint found for {Method} {Path} in project {ProjectSlug}", 
                httpMethod, endpointPath, project.Slug);
            
            // Return 404 for mock path that doesn't match any endpoint
            await WriteNotFoundResponse(context, project, endpointPath);
            stopwatch.Stop();
            
            // Log the request
            if (project.EnableLogging)
            {
                await LogRequestAsync(dbContext, requestLogs, notifier, project, null, context, endpointPath, 404, stopwatch.ElapsedMilliseconds, null, null);
            }
            
            return true;
        }

        _logger.LogInformation("Mock request matched: {Method} {Path} -> {EndpointName}", 
            httpMethod, endpointPath, matchingEndpoint.Name);

        // Apply delay if configured
        await ApplyDelayAsync(matchingEndpoint);

        // Get the response
        var response = GetResponse(matchingEndpoint);
        
        // Process the response body with template engine
        var requestContext = await BuildRequestContext(context, endpointPath, routeParams);
        var processedBody = _templateEngine.Render(response.Body ?? "{}", requestContext);

        // Write response
        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = response.ContentType ?? "application/json";

        // Add custom headers
        if (!string.IsNullOrEmpty(response.Headers))
        {
            try
            {
                var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Headers);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        context.Response.Headers[header.Key] = header.Value;
                    }
                }
            }
            catch (JsonException)
            {
                // Invalid headers JSON, ignore
            }
        }

        // Add CORS headers if enabled
        if (project.EnableCors)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD";
            context.Response.Headers["Access-Control-Allow-Headers"] = "*";
        }

        await context.Response.WriteAsync(processedBody);
        
        stopwatch.Stop();

        // Log the request if enabled
        if (project.EnableLogging)
        {
            await LogRequestAsync(dbContext, requestLogs, notifier, project, matchingEndpoint, context, endpointPath, 
                response.StatusCode, stopwatch.ElapsedMilliseconds, processedBody, response.Headers);
        }

        return true;
    }

    private MockEndpoint? FindMatchingEndpoint(
        ICollection<MockEndpoint> endpoints, 
        string method, 
        string path,
        out Dictionary<string, string> routeParams)
    {
        routeParams = new Dictionary<string, string>();

        // Sort endpoints by specificity (non-wildcard first, then by route length)
        var sortedEndpoints = endpoints
            .Where(e => e.IsActive)
            .OrderByDescending(e => !e.IsWildcard)
            .ThenByDescending(e => e.Route.Count(c => c == '/'))
            .ToList();

        foreach (var endpoint in sortedEndpoints)
        {
            // Check HTTP method
            if (!endpoint.Method.ToString().Equals(method, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Try to match route
            var matchResult = _routeMatcher.Match(endpoint.Route, path, endpoint.IsWildcard, endpoint.RegexPattern);
            if (matchResult.IsMatch)
            {
                routeParams = matchResult.Parameters;
                return endpoint;
            }
        }

        return null;
    }

    private MockResponse GetResponse(MockEndpoint endpoint)
    {
        // Get default response or first available
        var response = endpoint.Responses.FirstOrDefault(r => r.IsDefault) 
                      ?? endpoint.Responses.FirstOrDefault();

        if (response == null)
        {
            // Create default response if none exists
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "{\"message\": \"No response configured\"}"
            };
        }

        return response;
    }

    private async Task ApplyDelayAsync(MockEndpoint endpoint)
    {
        if (endpoint.DelayMin > 0 || endpoint.DelayMax > 0)
        {
            var min = endpoint.DelayMin ?? 0;
            var max = endpoint.DelayMax ?? min;
            var delay = min == max ? min : Random.Shared.Next(min, max + 1);
            
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
        }
    }

    private async Task<MockRequestContext> BuildRequestContext(
        HttpContext context, 
        string path,
        Dictionary<string, string> routeParams)
    {
        var queryParams = context.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        var headers = context.Request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        string? body = null;
        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        return new MockRequestContext
        {
            Path = path,
            Method = context.Request.Method,
            QueryParams = queryParams,
            Headers = headers,
            RouteParams = routeParams,
            Body = body
        };
    }

    private async Task WriteNotFoundResponse(HttpContext context, MockProject project, string path)
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "application/json";

        if (project.EnableCors)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        }
        
        var response = new
        {
            error = "Endpoint not found",
            project = project.Slug,
            path = path,
            availableEndpoints = project.Endpoints
                .Where(e => e.IsActive)
                .Select(e => new { method = e.Method.ToString(), route = e.Route })
                .ToList(),
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private async Task LogRequestAsync(
        TDbContext dbContext,
        DbSet<RequestLog> requestLogs,
        IRequestLogNotifier? notifier,
        MockProject project,
        MockEndpoint? endpoint,
        HttpContext context,
        string path,
        int statusCode,
        long durationMs,
        string? responseBody,
        string? responseHeaders)
    {
        try
        {
            string? requestBody = null;
            if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
            }

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var timestamp = DateTime.UtcNow;

            var log = new RequestLog
            {
                ProjectId = project.Id,
                EndpointId = endpoint?.Id,
                Method = context.Request.Method,
                Path = path,
                QueryString = context.Request.QueryString.Value,
                RequestHeaders = JsonSerializer.Serialize(
                    context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
                RequestBody = requestBody,
                ResponseStatusCode = statusCode,
                ResponseHeaders = responseHeaders,
                ResponseBody = responseBody,
                DurationMs = durationMs,
                ClientIp = clientIp,
                UserAgent = userAgent,
                IsMatched = endpoint != null,
                MatchedRoute = endpoint?.Route,
                CreatedAt = timestamp
            };

            requestLogs.Add(log);
            await dbContext.SaveChangesAsync();

            // Send real-time notification via SignalR
            if (notifier != null)
            {
                await notifier.NotifyRequestReceivedAsync(new RequestLogNotification
                {
                    ProjectId = project.Id,
                    EndpointId = endpoint?.Id,
                    Method = context.Request.Method,
                    Path = path,
                    QueryString = context.Request.QueryString.Value,
                    StatusCode = statusCode,
                    DurationMs = durationMs,
                    IsMatched = endpoint != null,
                    ClientIp = clientIp,
                    Timestamp = timestamp
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log request");
        }
    }
}

// Extension method to add middleware
public static class MockRoutingMiddlewareExtensions
{
    public static IApplicationBuilder UseMockRouting<TDbContext>(this IApplicationBuilder app) 
        where TDbContext : DbContext
    {
        return app.UseMiddleware<MockRoutingMiddleware<TDbContext>>();
    }
}
