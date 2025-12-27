using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MockHub.Application.Common;
using MockHub.Application.DTOs.Response;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.Infrastructure.Data;

namespace MockHub.Infrastructure.Services;

public class MockResponseService : IMockResponseService
{
    private readonly MockHubDbContext _context;

    public MockResponseService(MockHubDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MockResponseDto>>> GetByEndpointAsync(Guid endpointId)
    {
        var responses = await _context.MockResponses
            .Where(r => r.EndpointId == endpointId)
            .OrderBy(r => r.Order)
            .Select(r => MapToDto(r))
            .ToListAsync();

        return Result<List<MockResponseDto>>.Success(responses);
    }

    public async Task<Result<MockResponseDto>> GetByIdAsync(Guid responseId)
    {
        var response = await _context.MockResponses.FindAsync(responseId);
        if (response == null)
        {
            return Result<MockResponseDto>.Failure("Response not found");
        }

        return Result<MockResponseDto>.Success(MapToDto(response));
    }

    public async Task<Result<MockResponseDto>> CreateAsync(CreateMockResponseDto dto)
    {
        var endpoint = await _context.MockEndpoints.FindAsync(dto.EndpointId);
        if (endpoint == null)
        {
            return Result<MockResponseDto>.Failure("Endpoint not found");
        }

        var maxOrder = await _context.MockResponses
            .Where(r => r.EndpointId == dto.EndpointId)
            .MaxAsync(r => (int?)r.Order) ?? -1;

        var response = new MockResponse
        {
            EndpointId = dto.EndpointId,
            Name = dto.Name,
            Description = dto.Description,
            StatusCode = dto.StatusCode,
            Body = dto.Body,
            ContentType = dto.ContentType,
            IsDefault = dto.IsDefault,
            Headers = dto.Headers != null ? JsonSerializer.Serialize(dto.Headers) : null,
            Condition = dto.Condition,
            ConditionExpression = dto.ConditionExpression,
            Order = maxOrder + 1
        };

        // If this is set as default, unset other defaults
        if (dto.IsDefault)
        {
            var existingDefaults = await _context.MockResponses
                .Where(r => r.EndpointId == dto.EndpointId && r.IsDefault)
                .ToListAsync();

            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
            }
        }

        _context.MockResponses.Add(response);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(response.Id);
    }

    public async Task<Result<MockResponseDto>> UpdateAsync(Guid responseId, UpdateMockResponseDto dto)
    {
        var response = await _context.MockResponses.FindAsync(responseId);
        if (response == null)
        {
            return Result<MockResponseDto>.Failure("Response not found");
        }

        response.Name = dto.Name;
        response.Description = dto.Description;
        response.StatusCode = dto.StatusCode;
        response.Body = dto.Body;
        response.ContentType = dto.ContentType;
        response.Order = dto.Order;
        response.IsActive = dto.IsActive;
        response.Headers = dto.Headers != null ? JsonSerializer.Serialize(dto.Headers) : null;
        response.Condition = dto.Condition;
        response.ConditionExpression = dto.ConditionExpression;
        response.IsFileResponse = dto.IsFileResponse;
        response.FilePath = dto.FilePath;
        response.FileName = dto.FileName;
        response.UpdatedAt = DateTime.UtcNow;

        // Handle default flag
        if (dto.IsDefault && !response.IsDefault)
        {
            var existingDefaults = await _context.MockResponses
                .Where(r => r.EndpointId == response.EndpointId && r.IsDefault && r.Id != responseId)
                .ToListAsync();

            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
            }
        }
        response.IsDefault = dto.IsDefault;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(responseId);
    }

    public async Task<Result> DeleteAsync(Guid responseId)
    {
        var response = await _context.MockResponses.FindAsync(responseId);
        if (response == null)
        {
            return Result.Failure("Response not found");
        }

        _context.MockResponses.Remove(response);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> SetDefaultAsync(Guid responseId)
    {
        var response = await _context.MockResponses.FindAsync(responseId);
        if (response == null)
        {
            return Result.Failure("Response not found");
        }

        var allResponses = await _context.MockResponses
            .Where(r => r.EndpointId == response.EndpointId)
            .ToListAsync();

        foreach (var r in allResponses)
        {
            r.IsDefault = r.Id == responseId;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ReorderAsync(Guid endpointId, List<Guid> responseIds)
    {
        var responses = await _context.MockResponses
            .Where(r => r.EndpointId == endpointId)
            .ToListAsync();

        for (int i = 0; i < responseIds.Count; i++)
        {
            var response = responses.FirstOrDefault(r => r.Id == responseIds[i]);
            if (response != null)
            {
                response.Order = i;
            }
        }

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    private static MockResponseDto MapToDto(MockResponse r)
    {
        Dictionary<string, string>? headers = null;
        if (!string.IsNullOrEmpty(r.Headers))
        {
            try
            {
                headers = JsonSerializer.Deserialize<Dictionary<string, string>>(r.Headers);
            }
            catch { }
        }

        return new MockResponseDto
        {
            Id = r.Id,
            EndpointId = r.EndpointId,
            Name = r.Name,
            Description = r.Description,
            StatusCode = r.StatusCode,
            Body = r.Body,
            ContentType = r.ContentType,
            IsDefault = r.IsDefault,
            Order = r.Order,
            IsActive = r.IsActive,
            Headers = headers,
            Condition = r.Condition,
            ConditionExpression = r.ConditionExpression,
            IsFileResponse = r.IsFileResponse,
            FilePath = r.FilePath,
            FileName = r.FileName,
            CreatedAt = r.CreatedAt
        };
    }
}


