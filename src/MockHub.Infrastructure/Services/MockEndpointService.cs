using Microsoft.EntityFrameworkCore;
using MockHub.Application.Common;
using MockHub.Application.DTOs.Endpoint;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.Infrastructure.Data;

namespace MockHub.Infrastructure.Services;

public class MockEndpointService : IMockEndpointService
{
    private readonly MockHubDbContext _context;

    public MockEndpointService(MockHubDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MockEndpointDto>>> GetByProjectAsync(Guid projectId)
    {
        var endpoints = await _context.MockEndpoints
            .Include(e => e.Responses)
            .Where(e => e.ProjectId == projectId)
            .OrderBy(e => e.Order)
            .Select(e => MapToDto(e))
            .ToListAsync();

        return Result<List<MockEndpointDto>>.Success(endpoints);
    }

    public async Task<Result<MockEndpointDto>> GetByIdAsync(Guid endpointId)
    {
        var endpoint = await _context.MockEndpoints
            .Include(e => e.Responses)
            .FirstOrDefaultAsync(e => e.Id == endpointId);

        if (endpoint == null)
        {
            return Result<MockEndpointDto>.Failure("Endpoint not found");
        }

        return Result<MockEndpointDto>.Success(MapToDto(endpoint));
    }

    public async Task<Result<MockEndpointDto>> CreateAsync(CreateMockEndpointDto dto)
    {
        return await CreateWithResponseAsync(dto, 200, "application/json", "{}", null);
    }

    public async Task<Result<MockEndpointDto>> CreateWithResponseAsync(
        CreateMockEndpointDto dto,
        int statusCode,
        string contentType,
        string? body,
        string? headers)
    {
        var project = await _context.MockProjects.FindAsync(dto.ProjectId);
        if (project == null)
        {
            return Result<MockEndpointDto>.Failure("Project not found");
        }

        var maxOrder = await _context.MockEndpoints
            .Where(e => e.ProjectId == dto.ProjectId)
            .MaxAsync(e => (int?)e.Order) ?? -1;

        var endpoint = new MockEndpoint
        {
            ProjectId = dto.ProjectId,
            Name = dto.Name,
            Description = dto.Description,
            Route = dto.Route,
            Method = dto.Method,
            IsWildcard = dto.IsWildcard,
            RegexPattern = dto.RegexPattern,
            ResponseMode = dto.ResponseMode,
            DelayMin = dto.DelayMin,
            DelayMax = dto.DelayMax,
            Order = maxOrder + 1
        };

        _context.MockEndpoints.Add(endpoint);

        // Create response with provided values
        var defaultResponse = new MockResponse
        {
            EndpointId = endpoint.Id,
            Name = "Default Response",
            StatusCode = statusCode,
            Body = body ?? "{}",
            ContentType = contentType,
            Headers = headers,
            IsDefault = true,
            Order = 0
        };

        _context.MockResponses.Add(defaultResponse);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(endpoint.Id);
    }

    public async Task<Result<MockEndpointDto>> UpdateAsync(Guid endpointId, UpdateMockEndpointDto dto)
    {
        var endpoint = await _context.MockEndpoints.FindAsync(endpointId);
        if (endpoint == null)
        {
            return Result<MockEndpointDto>.Failure("Endpoint not found");
        }

        endpoint.Name = dto.Name;
        endpoint.Description = dto.Description;
        endpoint.Route = dto.Route;
        endpoint.Method = dto.Method;
        endpoint.IsActive = dto.IsActive;
        endpoint.Order = dto.Order;
        endpoint.IsWildcard = dto.IsWildcard;
        endpoint.RegexPattern = dto.RegexPattern;
        endpoint.ResponseMode = dto.ResponseMode;
        endpoint.DelayMin = dto.DelayMin;
        endpoint.DelayMax = dto.DelayMax;
        endpoint.CallbackUrl = dto.CallbackUrl;
        endpoint.EnableCallback = dto.EnableCallback;
        endpoint.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(endpointId);
    }

    public async Task<Result<MockEndpointDto>> UpdateWithResponseAsync(
        Guid endpointId,
        UpdateMockEndpointDto dto,
        int statusCode,
        string contentType,
        string? body,
        string? headers)
    {
        var endpoint = await _context.MockEndpoints
            .Include(e => e.Responses)
            .FirstOrDefaultAsync(e => e.Id == endpointId);
            
        if (endpoint == null)
        {
            return Result<MockEndpointDto>.Failure("Endpoint not found");
        }

        // Update endpoint
        endpoint.Name = dto.Name;
        endpoint.Description = dto.Description;
        endpoint.Route = dto.Route;
        endpoint.Method = dto.Method;
        endpoint.IsActive = dto.IsActive;
        endpoint.Order = dto.Order;
        endpoint.IsWildcard = dto.IsWildcard;
        endpoint.RegexPattern = dto.RegexPattern;
        endpoint.ResponseMode = dto.ResponseMode;
        endpoint.DelayMin = dto.DelayMin;
        endpoint.DelayMax = dto.DelayMax;
        endpoint.CallbackUrl = dto.CallbackUrl;
        endpoint.EnableCallback = dto.EnableCallback;
        endpoint.UpdatedAt = DateTime.UtcNow;

        // Update or create default response
        var defaultResponse = endpoint.Responses.FirstOrDefault(r => r.IsDefault);
        if (defaultResponse != null)
        {
            defaultResponse.StatusCode = statusCode;
            defaultResponse.ContentType = contentType;
            defaultResponse.Body = body ?? "{}";
            defaultResponse.Headers = headers;
            defaultResponse.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newResponse = new MockResponse
            {
                EndpointId = endpoint.Id,
                Name = "Default Response",
                StatusCode = statusCode,
                Body = body ?? "{}",
                ContentType = contentType,
                Headers = headers,
                IsDefault = true,
                Order = 0
            };
            _context.MockResponses.Add(newResponse);
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(endpointId);
    }

    public async Task<Result> DeleteAsync(Guid endpointId)
    {
        var endpoint = await _context.MockEndpoints.FindAsync(endpointId);
        if (endpoint == null)
        {
            return Result.Failure("Endpoint not found");
        }

        _context.MockEndpoints.Remove(endpoint);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ReorderAsync(Guid projectId, List<Guid> endpointIds)
    {
        var endpoints = await _context.MockEndpoints
            .Where(e => e.ProjectId == projectId)
            .ToListAsync();

        for (int i = 0; i < endpointIds.Count; i++)
        {
            var endpoint = endpoints.FirstOrDefault(e => e.Id == endpointIds[i]);
            if (endpoint != null)
            {
                endpoint.Order = i;
            }
        }

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<MockEndpointDto>> DuplicateAsync(Guid endpointId)
    {
        var original = await _context.MockEndpoints
            .Include(e => e.Responses)
            .Include(e => e.ValidationRules)
            .FirstOrDefaultAsync(e => e.Id == endpointId);

        if (original == null)
        {
            return Result<MockEndpointDto>.Failure("Endpoint not found");
        }

        var maxOrder = await _context.MockEndpoints
            .Where(e => e.ProjectId == original.ProjectId)
            .MaxAsync(e => (int?)e.Order) ?? -1;

        var duplicate = new MockEndpoint
        {
            ProjectId = original.ProjectId,
            Name = $"{original.Name} (Copy)",
            Description = original.Description,
            Route = original.Route,
            Method = original.Method,
            IsWildcard = original.IsWildcard,
            RegexPattern = original.RegexPattern,
            ResponseMode = original.ResponseMode,
            DelayMin = original.DelayMin,
            DelayMax = original.DelayMax,
            Order = maxOrder + 1
        };

        _context.MockEndpoints.Add(duplicate);

        foreach (var response in original.Responses)
        {
            var duplicateResponse = new MockResponse
            {
                EndpointId = duplicate.Id,
                Name = response.Name,
                Description = response.Description,
                StatusCode = response.StatusCode,
                Body = response.Body,
                ContentType = response.ContentType,
                Headers = response.Headers,
                IsDefault = response.IsDefault,
                Order = response.Order,
                Condition = response.Condition,
                ConditionExpression = response.ConditionExpression
            };
            _context.MockResponses.Add(duplicateResponse);
        }

        foreach (var rule in original.ValidationRules)
        {
            var duplicateRule = new ValidationRule
            {
                EndpointId = duplicate.Id,
                ParameterName = rule.ParameterName,
                Location = rule.Location,
                IsRequired = rule.IsRequired,
                DataType = rule.DataType,
                RegexPattern = rule.RegexPattern,
                MinValue = rule.MinValue,
                MaxValue = rule.MaxValue,
                DefaultValue = rule.DefaultValue,
                ErrorMessage = rule.ErrorMessage
            };
            _context.ValidationRules.Add(duplicateRule);
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(duplicate.Id);
    }

    private static MockEndpointDto MapToDto(MockEndpoint e)
    {
        return new MockEndpointDto
        {
            Id = e.Id,
            ProjectId = e.ProjectId,
            Name = e.Name,
            Description = e.Description,
            Route = e.Route,
            Method = e.Method,
            IsActive = e.IsActive,
            Order = e.Order,
            IsWildcard = e.IsWildcard,
            RegexPattern = e.RegexPattern,
            ResponseMode = e.ResponseMode,
            DelayMin = e.DelayMin,
            DelayMax = e.DelayMax,
            ResponseCount = e.Responses.Count,
            CreatedAt = e.CreatedAt,
            Responses = e.Responses.Select(r => new MockResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                StatusCode = r.StatusCode,
                Body = r.Body,
                ContentType = r.ContentType,
                Headers = r.Headers,
                IsDefault = r.IsDefault
            }).ToList()
        };
    }
}


