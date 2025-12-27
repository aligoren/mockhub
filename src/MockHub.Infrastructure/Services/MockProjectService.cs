using Microsoft.EntityFrameworkCore;
using MockHub.Application.Common;
using MockHub.Application.DTOs.Project;
using MockHub.Application.Helpers;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.Infrastructure.Data;

namespace MockHub.Infrastructure.Services;

public class MockProjectService : IMockProjectService
{
    private readonly MockHubDbContext _context;
    private readonly ITeamService _teamService;

    public MockProjectService(MockHubDbContext context, ITeamService teamService)
    {
        _context = context;
        _teamService = teamService;
    }

    public async Task<Result<List<MockProjectDto>>> GetAllAsync(string userId)
    {
        // First materialize to avoid SQLite APPLY limitations
        var projects = await _context.MockProjects
            .Include(p => p.Team)
                .ThenInclude(t => t!.Members)
            .Include(p => p.Endpoints)
            .Where(p => p.UserId == userId || 
                       (p.TeamId != null && p.Team!.Members.Any(m => m.UserId == userId && m.IsActive)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Result<List<MockProjectDto>>.Success(projects.Select(MapToDto).ToList());
    }

    public async Task<Result<List<MockProjectDto>>> GetByTeamAsync(Guid teamId)
    {
        // First materialize to avoid SQLite APPLY limitations
        var projects = await _context.MockProjects
            .Include(p => p.Team)
            .Include(p => p.Endpoints)
            .Where(p => p.TeamId == teamId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Result<List<MockProjectDto>>.Success(projects.Select(MapToDto).ToList());
    }

    public async Task<Result<MockProjectDto>> GetByIdAsync(Guid projectId)
    {
        var project = await _context.MockProjects
            .Include(p => p.Team)
            .Include(p => p.Endpoints)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            return Result<MockProjectDto>.Failure("Project not found");
        }

        return Result<MockProjectDto>.Success(MapToDto(project));
    }

    public async Task<Result<MockProject>> GetByIdWithEndpointsAsync(Guid projectId)
    {
        var project = await _context.MockProjects
            .Include(p => p.Endpoints)
                .ThenInclude(e => e.Responses)
            .Include(p => p.Endpoints)
                .ThenInclude(e => e.ValidationRules)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            return Result<MockProject>.Failure("Project not found");
        }

        return Result<MockProject>.Success(project);
    }

    public async Task<Result<MockProjectDto>> GetBySlugAsync(string slug, string? teamSlug = null)
    {
        MockProject? project;
        
        if (string.IsNullOrEmpty(teamSlug))
        {
            // Personal project - slug must be unique among personal projects
            project = await _context.MockProjects
                .Include(p => p.Team)
                .Include(p => p.Endpoints)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.TeamId == null);
        }
        else
        {
            // Team project - find by team slug and project slug
            project = await _context.MockProjects
                .Include(p => p.Team)
                .Include(p => p.Endpoints)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Team != null && p.Team.Slug == teamSlug);
        }

        if (project == null)
        {
            return Result<MockProjectDto>.Failure("Project not found");
        }

        return Result<MockProjectDto>.Success(MapToDto(project));
    }

    public async Task<Result<MockProject>> GetBySlugWithEndpointsAsync(string slug, string? teamSlug = null)
    {
        MockProject? project;
        
        if (string.IsNullOrEmpty(teamSlug))
        {
            project = await _context.MockProjects
                .Include(p => p.Team)
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.Responses)
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.ValidationRules)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.TeamId == null);
        }
        else
        {
            project = await _context.MockProjects
                .Include(p => p.Team)
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.Responses)
                .Include(p => p.Endpoints)
                    .ThenInclude(e => e.ValidationRules)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Team != null && p.Team.Slug == teamSlug);
        }

        if (project == null)
        {
            return Result<MockProject>.Failure("Project not found");
        }

        return Result<MockProject>.Success(project);
    }

    public async Task<Result<MockProjectDto>> CreateAsync(string userId, CreateMockProjectDto dto)
    {
        // Generate slug
        string slug;
        if (!string.IsNullOrEmpty(dto.Slug))
        {
            slug = SlugHelper.GenerateSlug(dto.Slug);
        }
        else
        {
            slug = SlugHelper.GenerateSlug(dto.Name);
        }

        // Ensure slug uniqueness within scope
        if (dto.TeamId.HasValue)
        {
            // Team project - unique within team
            slug = SlugHelper.GenerateUniqueSlug(slug, s => 
                _context.MockProjects.Any(p => p.TeamId == dto.TeamId && p.Slug == s));
        }
        else
        {
            // Personal project - unique among personal projects
            slug = SlugHelper.GenerateUniqueSlug(slug, s => 
                _context.MockProjects.Any(p => p.TeamId == null && p.Slug == s));
        }

        var project = new MockProject
        {
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            TeamId = dto.TeamId,
            UserId = dto.TeamId == null ? userId : null,
            DefaultDelay = dto.DefaultDelay,
            EnableCors = dto.EnableCors,
            EnableLogging = dto.EnableLogging,
            CreatedBy = userId
        };

        _context.MockProjects.Add(project);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(project.Id);
    }

    public async Task<Result<MockProjectDto>> UpdateAsync(Guid projectId, UpdateMockProjectDto dto)
    {
        var project = await _context.MockProjects
            .Include(p => p.Team)
            .FirstOrDefaultAsync(p => p.Id == projectId);
            
        if (project == null)
        {
            return Result<MockProjectDto>.Failure("Project not found");
        }

        // Regenerate slug if name changed
        if (project.Name != dto.Name)
        {
            var newSlug = SlugHelper.GenerateSlug(dto.Name);
            // Uniqueness check (according to scope)
            if (project.TeamId.HasValue)
            {
                newSlug = SlugHelper.GenerateUniqueSlug(newSlug, s => 
                    _context.MockProjects.Any(p => p.Id != projectId && p.TeamId == project.TeamId && p.Slug == s));
            }
            else
            {
                newSlug = SlugHelper.GenerateUniqueSlug(newSlug, s => 
                    _context.MockProjects.Any(p => p.Id != projectId && p.TeamId == null && p.Slug == s));
            }
            project.Slug = newSlug;
        }

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.IsActive = dto.IsActive;
        project.DefaultDelay = dto.DefaultDelay;
        project.EnableCors = dto.EnableCors;
        project.EnableLogging = dto.EnableLogging;
        project.EnableLatencySimulation = dto.EnableLatencySimulation;
        project.GlobalLatencyMin = dto.GlobalLatencyMin;
        project.GlobalLatencyMax = dto.GlobalLatencyMax;
        project.EnableJwtValidation = dto.EnableJwtValidation;
        project.JwtSecret = dto.JwtSecret;
        project.JwtIssuer = dto.JwtIssuer;
        project.JwtAudience = dto.JwtAudience;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(projectId);
    }

    public async Task<Result> DeleteAsync(Guid projectId)
    {
        var project = await _context.MockProjects.FindAsync(projectId);
        if (project == null)
        {
            return Result.Failure("Project not found");
        }

        _context.MockProjects.Remove(project);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<bool> IsSlugAvailableAsync(string slug, Guid? teamId)
    {
        if (teamId.HasValue)
        {
            return !await _context.MockProjects.AnyAsync(p => p.TeamId == teamId && p.Slug == slug);
        }
        return !await _context.MockProjects.AnyAsync(p => p.TeamId == null && p.Slug == slug);
    }

    public async Task<bool> CanUserAccessProjectAsync(Guid projectId, string userId)
    {
        var project = await _context.MockProjects
            .Include(p => p.Team)
            .ThenInclude(t => t!.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return false;

        // Personal project
        if (project.UserId == userId) return true;

        // Team project
        if (project.TeamId != null)
        {
            return project.Team?.Members.Any(m => m.UserId == userId && m.IsActive) ?? false;
        }

        return false;
    }

    private static MockProjectDto MapToDto(MockProject p)
    {
        return new MockProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            IsActive = p.IsActive,
            TeamId = p.TeamId,
            TeamName = p.Team?.Name,
            TeamSlug = p.Team?.Slug,
            UserId = p.UserId,
            EndpointCount = p.Endpoints.Count,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            DefaultDelay = p.DefaultDelay,
            EnableCors = p.EnableCors,
            EnableLogging = p.EnableLogging,
            EnableJwtValidation = p.EnableJwtValidation
        };
    }
}
