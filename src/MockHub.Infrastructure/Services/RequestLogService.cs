using Microsoft.EntityFrameworkCore;
using MockHub.Application.Common;
using MockHub.Application.Interfaces;
using MockHub.Infrastructure.Data;

namespace MockHub.Infrastructure.Services;

public class RequestLogService : IRequestLogService
{
    private readonly MockHubDbContext _context;

    public RequestLogService(MockHubDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<RequestLogDetailDto>>> GetAllLogsAsync(int page = 1, int pageSize = 100, Guid? projectId = null)
    {
        var query = _context.RequestLogs
            .Include(l => l.Project)
            .Include(l => l.Endpoint)
            .AsQueryable();

        if (projectId.HasValue)
        {
            query = query.Where(l => l.ProjectId == projectId.Value);
        }

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = logs.Select(l => new RequestLogDetailDto
        {
            Id = l.Id,
            ProjectId = l.ProjectId,
            ProjectName = l.Project?.Name ?? "Unknown",
            ProjectSlug = l.Project?.Slug ?? "",
            EndpointId = l.EndpointId,
            EndpointName = l.Endpoint?.Name,
            EndpointRoute = l.Endpoint?.Route,
            Method = l.Method,
            Path = l.Path,
            QueryString = l.QueryString,
            RequestHeaders = l.RequestHeaders,
            RequestBody = l.RequestBody,
            ResponseStatusCode = l.ResponseStatusCode,
            ResponseHeaders = l.ResponseHeaders,
            ResponseBody = l.ResponseBody,
            DurationMs = l.DurationMs,
            ClientIp = l.ClientIp,
            UserAgent = l.UserAgent,
            IsMatched = l.IsMatched,
            MatchedRoute = l.MatchedRoute,
            ErrorMessage = l.ErrorMessage,
            CreatedAt = l.CreatedAt
        }).ToList();

        return Result<List<RequestLogDetailDto>>.Success(dtos);
    }

    public async Task<Result<List<RequestLogDetailDto>>> GetUserLogsAsync(string userId, int page = 1, int pageSize = 100, Guid? projectId = null)
    {
        // Get user's personal project IDs
        var personalProjectIds = await _context.MockProjects
            .Where(p => p.UserId == userId && p.TeamId == null)
            .Select(p => p.Id)
            .ToListAsync();

        // Get user's team project IDs
        var teamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId)
            .Select(tm => tm.TeamId)
            .ToListAsync();

        var teamProjectIds = await _context.MockProjects
            .Where(p => p.TeamId.HasValue && teamIds.Contains(p.TeamId.Value))
            .Select(p => p.Id)
            .ToListAsync();

        var allowedProjectIds = personalProjectIds.Concat(teamProjectIds).ToHashSet();

        var query = _context.RequestLogs
            .Include(l => l.Project)
            .Include(l => l.Endpoint)
            .Where(l => allowedProjectIds.Contains(l.ProjectId));

        if (projectId.HasValue)
        {
            query = query.Where(l => l.ProjectId == projectId.Value);
        }

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = logs.Select(l => new RequestLogDetailDto
        {
            Id = l.Id,
            ProjectId = l.ProjectId,
            ProjectName = l.Project?.Name ?? "Unknown",
            ProjectSlug = l.Project?.Slug ?? "",
            EndpointId = l.EndpointId,
            EndpointName = l.Endpoint?.Name,
            EndpointRoute = l.Endpoint?.Route,
            Method = l.Method,
            Path = l.Path,
            QueryString = l.QueryString,
            RequestHeaders = l.RequestHeaders,
            RequestBody = l.RequestBody,
            ResponseStatusCode = l.ResponseStatusCode,
            ResponseHeaders = l.ResponseHeaders,
            ResponseBody = l.ResponseBody,
            DurationMs = l.DurationMs,
            ClientIp = l.ClientIp,
            UserAgent = l.UserAgent,
            IsMatched = l.IsMatched,
            MatchedRoute = l.MatchedRoute,
            ErrorMessage = l.ErrorMessage,
            CreatedAt = l.CreatedAt
        }).ToList();

        return Result<List<RequestLogDetailDto>>.Success(dtos);
    }

    public async Task<Result<List<RequestLogDetailDto>>> GetEndpointLogsAsync(Guid endpointId, int page = 1, int pageSize = 50)
    {
        var logs = await _context.RequestLogs
            .Include(l => l.Project)
            .Include(l => l.Endpoint)
            .Where(l => l.EndpointId == endpointId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = logs.Select(l => new RequestLogDetailDto
        {
            Id = l.Id,
            ProjectId = l.ProjectId,
            ProjectName = l.Project?.Name ?? "Unknown",
            ProjectSlug = l.Project?.Slug ?? "",
            EndpointId = l.EndpointId,
            EndpointName = l.Endpoint?.Name,
            EndpointRoute = l.Endpoint?.Route,
            Method = l.Method,
            Path = l.Path,
            QueryString = l.QueryString,
            RequestHeaders = l.RequestHeaders,
            RequestBody = l.RequestBody,
            ResponseStatusCode = l.ResponseStatusCode,
            ResponseHeaders = l.ResponseHeaders,
            ResponseBody = l.ResponseBody,
            DurationMs = l.DurationMs,
            ClientIp = l.ClientIp,
            UserAgent = l.UserAgent,
            IsMatched = l.IsMatched,
            MatchedRoute = l.MatchedRoute,
            ErrorMessage = l.ErrorMessage,
            CreatedAt = l.CreatedAt
        }).ToList();

        return Result<List<RequestLogDetailDto>>.Success(dtos);
    }

    public async Task<Result<RequestLogDetailDto>> GetByIdAsync(Guid logId)
    {
        var log = await _context.RequestLogs
            .Include(l => l.Project)
            .Include(l => l.Endpoint)
            .FirstOrDefaultAsync(l => l.Id == logId);

        if (log == null)
        {
            return Result<RequestLogDetailDto>.Failure("Log not found");
        }

        var dto = new RequestLogDetailDto
        {
            Id = log.Id,
            ProjectId = log.ProjectId,
            ProjectName = log.Project?.Name ?? "Unknown",
            ProjectSlug = log.Project?.Slug ?? "",
            EndpointId = log.EndpointId,
            EndpointName = log.Endpoint?.Name,
            EndpointRoute = log.Endpoint?.Route,
            Method = log.Method,
            Path = log.Path,
            QueryString = log.QueryString,
            RequestHeaders = log.RequestHeaders,
            RequestBody = log.RequestBody,
            ResponseStatusCode = log.ResponseStatusCode,
            ResponseHeaders = log.ResponseHeaders,
            ResponseBody = log.ResponseBody,
            DurationMs = log.DurationMs,
            ClientIp = log.ClientIp,
            UserAgent = log.UserAgent,
            IsMatched = log.IsMatched,
            MatchedRoute = log.MatchedRoute,
            ErrorMessage = log.ErrorMessage,
            CreatedAt = log.CreatedAt
        };

        return Result<RequestLogDetailDto>.Success(dto);
    }

    public async Task<Result> DeleteLogAsync(Guid logId, string userId, bool isAdmin)
    {
        var log = await _context.RequestLogs
            .Include(l => l.Project)
            .FirstOrDefaultAsync(l => l.Id == logId);

        if (log == null)
        {
            return Result.Failure("Log not found");
        }

        // Check permission
        if (!isAdmin)
        {
            var hasPermission = await HasProjectPermissionAsync(log.ProjectId, userId);
            if (!hasPermission)
            {
                return Result.Failure("You don't have permission to delete this log");
            }
        }

        _context.RequestLogs.Remove(log);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteProjectLogsAsync(Guid projectId, string userId, bool isAdmin)
    {
        // Check permission
        if (!isAdmin)
        {
            var hasPermission = await HasProjectPermissionAsync(projectId, userId);
            if (!hasPermission)
            {
                return Result.Failure("You don't have permission to delete logs for this project");
            }
        }

        var logs = await _context.RequestLogs
            .Where(l => l.ProjectId == projectId)
            .ToListAsync();

        _context.RequestLogs.RemoveRange(logs);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAllLogsAsync()
    {
        var logs = await _context.RequestLogs.ToListAsync();
        _context.RequestLogs.RemoveRange(logs);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<int> GetLogCountAsync(string? userId = null, bool isAdmin = false)
    {
        if (isAdmin || string.IsNullOrEmpty(userId))
        {
            return await _context.RequestLogs.CountAsync();
        }

        // Get user's allowed project IDs
        var personalProjectIds = await _context.MockProjects
            .Where(p => p.UserId == userId && p.TeamId == null)
            .Select(p => p.Id)
            .ToListAsync();

        var teamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId)
            .Select(tm => tm.TeamId)
            .ToListAsync();

        var teamProjectIds = await _context.MockProjects
            .Where(p => p.TeamId.HasValue && teamIds.Contains(p.TeamId.Value))
            .Select(p => p.Id)
            .ToListAsync();

        var allowedProjectIds = personalProjectIds.Concat(teamProjectIds).ToHashSet();

        return await _context.RequestLogs
            .Where(l => allowedProjectIds.Contains(l.ProjectId))
            .CountAsync();
    }

    private async Task<bool> HasProjectPermissionAsync(Guid projectId, string userId)
    {
        var project = await _context.MockProjects
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            return false;
        }

        // Check if user owns the project
        if (project.UserId == userId && project.TeamId == null)
        {
            return true;
        }

        // Check if user is a member of the project's team
        if (project.TeamId.HasValue)
        {
            var isMember = await _context.TeamMembers
                .AnyAsync(tm => tm.TeamId == project.TeamId.Value && tm.UserId == userId);
            return isMember;
        }

        return false;
    }
}

