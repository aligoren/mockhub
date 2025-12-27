using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockHub.Application.Common;
using MockHub.Application.DTOs.Team;
using MockHub.Application.Helpers;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.Domain.Enums;
using MockHub.Infrastructure.Data;

namespace MockHub.Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly MockHubDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeamService(MockHubDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<List<TeamDto>>> GetUserTeamsAsync(string userId)
    {
        // First, get teams and materialize them to avoid SQLite APPLY limitations
        var teams = await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Projects)
                .ThenInclude(p => p.Endpoints)
            .Where(t => t.Members.Any(m => m.UserId == userId && m.IsActive))
            .ToListAsync();

        // Now map to DTOs in memory
        var teamDtos = teams.Select(t => new TeamDto
        {
            Id = t.Id,
            Name = t.Name,
            Slug = t.Slug,
            Description = t.Description,
            LogoUrl = t.LogoUrl,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            MemberCount = t.Members.Count(m => m.IsActive),
            ProjectCount = t.Projects.Count(p => p.IsActive),
            Projects = t.Projects.Where(p => p.IsActive).Select(p => new TeamProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                IsActive = p.IsActive,
                EndpointCount = p.Endpoints.Count,
                MockUrl = $"/{t.Slug}/{p.Slug}"
            }).ToList()
        }).ToList();

        return Result<List<TeamDto>>.Success(teamDtos);
    }

    public async Task<Result<List<TeamDto>>> GetAllTeamsAsync()
    {
        var teams = await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Projects)
                .ThenInclude(p => p.Endpoints)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var teamDtos = teams.Select(t => new TeamDto
        {
            Id = t.Id,
            Name = t.Name,
            Slug = t.Slug,
            Description = t.Description,
            LogoUrl = t.LogoUrl,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            MemberCount = t.Members.Count(m => m.IsActive),
            ProjectCount = t.Projects.Count(p => p.IsActive),
            Projects = t.Projects.Where(p => p.IsActive).Select(p => new TeamProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                IsActive = p.IsActive,
                EndpointCount = p.Endpoints.Count,
                MockUrl = $"/{t.Slug}/{p.Slug}"
            }).ToList()
        }).ToList();

        return Result<List<TeamDto>>.Success(teamDtos);
    }

    public async Task<Result<TeamDto>> GetByIdAsync(Guid teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Projects)
                .ThenInclude(p => p.Endpoints)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            return Result<TeamDto>.Failure("Team not found");
        }

        return Result<TeamDto>.Success(MapToDto(team));
    }

    public async Task<Result<TeamDto>> GetBySlugAsync(string slug)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Projects)
                .ThenInclude(p => p.Endpoints)
            .FirstOrDefaultAsync(t => t.Slug == slug);

        if (team == null)
        {
            return Result<TeamDto>.Failure("Team not found");
        }

        return Result<TeamDto>.Success(MapToDto(team));
    }

    public async Task<Result<TeamDto>> CreateAsync(string userId, CreateTeamDto dto)
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

        // Ensure slug uniqueness
        slug = SlugHelper.GenerateUniqueSlug(slug, s => _context.Teams.Any(t => t.Slug == s));

        var team = new Team
        {
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            CreatedBy = userId
        };

        _context.Teams.Add(team);

        // Add creator as owner
        var teamMember = new TeamMember
        {
            TeamId = team.Id,
            UserId = userId,
            Role = TeamRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();

        return Result<TeamDto>.Success(new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Slug = team.Slug,
            Description = team.Description,
            LogoUrl = team.LogoUrl,
            IsActive = team.IsActive,
            CreatedAt = team.CreatedAt,
            MemberCount = 1,
            ProjectCount = 0,
            Projects = new List<TeamProjectDto>()
        });
    }

    public async Task<Result<TeamDto>> UpdateAsync(Guid teamId, UpdateTeamDto dto)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Projects)
                .ThenInclude(p => p.Endpoints)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            return Result<TeamDto>.Failure("Team not found");
        }

        // Regenerate slug if name changed
        if (team.Name != dto.Name)
        {
            var newSlug = SlugHelper.GenerateSlug(dto.Name);
            // Uniqueness check (excluding own ID)
            newSlug = SlugHelper.GenerateUniqueSlug(newSlug, s => 
                _context.Teams.Any(t => t.Id != teamId && t.Slug == s));
            team.Slug = newSlug;
        }

        team.Name = dto.Name;
        team.Description = dto.Description;
        team.LogoUrl = dto.LogoUrl;
        team.IsActive = dto.IsActive;
        team.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<TeamDto>.Success(MapToDto(team));
    }

    public async Task<Result> DeleteAsync(Guid teamId)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team == null)
        {
            return Result.Failure("Team not found");
        }

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<TeamMemberDto>>> GetMembersAsync(Guid teamId)
    {
        var members = await _context.TeamMembers
            .Where(m => m.TeamId == teamId && m.IsActive)
            .ToListAsync();

        var memberDtos = new List<TeamMemberDto>();
        foreach (var member in members)
        {
            var user = await _userManager.FindByIdAsync(member.UserId);
            if (user != null)
            {
                memberDtos.Add(new TeamMemberDto
                {
                    Id = member.Id,
                    UserId = member.UserId,
                    UserEmail = user.Email ?? string.Empty,
                    UserName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    Role = member.Role.ToString(),
                    JoinedAt = member.JoinedAt
                });
            }
        }

        return Result<List<TeamMemberDto>>.Success(memberDtos);
    }

    public async Task<Result<TeamMemberDto>> AddMemberAsync(Guid teamId, AddTeamMemberDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<TeamMemberDto>.Failure("User not found");
        }

        var existingMember = await _context.TeamMembers
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == user.Id);

        if (existingMember != null)
        {
            if (existingMember.IsActive)
            {
                return Result<TeamMemberDto>.Failure("User is already a member");
            }
            existingMember.IsActive = true;
            existingMember.Role = Enum.Parse<TeamRole>(dto.Role);
            existingMember.JoinedAt = DateTime.UtcNow;
        }
        else
        {
            var member = new TeamMember
            {
                TeamId = teamId,
                UserId = user.Id,
                Role = Enum.Parse<TeamRole>(dto.Role),
                JoinedAt = DateTime.UtcNow
            };
            _context.TeamMembers.Add(member);
        }

        await _context.SaveChangesAsync();

        var updatedMember = await _context.TeamMembers
            .FirstAsync(m => m.TeamId == teamId && m.UserId == user.Id);

        return Result<TeamMemberDto>.Success(new TeamMemberDto
        {
            Id = updatedMember.Id,
            UserId = user.Id,
            UserEmail = user.Email ?? string.Empty,
            UserName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Role = updatedMember.Role.ToString(),
            JoinedAt = updatedMember.JoinedAt
        });
    }

    public async Task<Result> RemoveMemberAsync(Guid teamId, string userId)
    {
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);

        if (member == null)
        {
            return Result.Failure("Member not found");
        }

        if (member.Role == TeamRole.Owner)
        {
            return Result.Failure("Cannot remove team owner");
        }

        member.IsActive = false;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateMemberRoleAsync(Guid teamId, string userId, string role)
    {
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);

        if (member == null)
        {
            return Result.Failure("Member not found");
        }

        if (!Enum.TryParse<TeamRole>(role, out var newRole))
        {
            return Result.Failure("Invalid role");
        }

        member.Role = newRole;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<bool> IsUserTeamMemberAsync(Guid teamId, string userId)
    {
        return await _context.TeamMembers
            .AnyAsync(m => m.TeamId == teamId && m.UserId == userId && m.IsActive);
    }

    public async Task<bool> IsUserTeamAdminAsync(Guid teamId, string userId)
    {
        return await _context.TeamMembers
            .AnyAsync(m => m.TeamId == teamId && 
                          m.UserId == userId && 
                          m.IsActive && 
                          (m.Role == TeamRole.Owner || m.Role == TeamRole.Admin));
    }

    public async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeTeamId = null)
    {
        if (excludeTeamId.HasValue)
        {
            return !await _context.Teams.AnyAsync(t => t.Id != excludeTeamId && t.Slug == slug);
        }
        return !await _context.Teams.AnyAsync(t => t.Slug == slug);
    }

    private TeamDto MapToDto(Team team)
    {
        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Slug = team.Slug,
            Description = team.Description,
            LogoUrl = team.LogoUrl,
            IsActive = team.IsActive,
            CreatedAt = team.CreatedAt,
            MemberCount = team.Members.Count(m => m.IsActive),
            ProjectCount = team.Projects.Count(p => p.IsActive),
            Projects = team.Projects.Where(p => p.IsActive).Select(p => new TeamProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                IsActive = p.IsActive,
                EndpointCount = p.Endpoints.Count,
                MockUrl = $"/{team.Slug}/{p.Slug}"
            }).ToList()
        };
    }
}
