using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockHub.Application.Common;
using MockHub.Application.DTOs.User;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.Domain.Enums;
using MockHub.Infrastructure.Data;

namespace MockHub.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly MockHubDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(MockHubDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> IsSetupRequiredAsync()
    {
        // Check if any admin user exists
        return !await _context.Users.AnyAsync(u => u.IsAdmin);
    }

    public async Task<Result<UserDto>> SetupAdminAsync(SetupAdminDto dto)
    {
        // Ensure no admin exists
        if (!await IsSetupRequiredAsync())
        {
            return Result<UserDto>.Failure("System is already set up. Admin user exists.");
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            return Result<UserDto>.Failure("Passwords do not match.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsAdmin = true,
            IsActive = true,
            EmailConfirmed = true // Admin doesn't need email confirmation
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<UserDto>.Failure(errors);
        }

        return await GetByIdAsync(user.Id);
    }

    public async Task<Result<List<UserDto>>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.TeamMemberships)
                .ThenInclude(tm => tm.Team)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        var userDtos = users.Select(MapToDto).ToList();
        return Result<List<UserDto>>.Success(userDtos);
    }

    public async Task<Result<UserDto>> GetByIdAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.TeamMemberships)
                .ThenInclude(tm => tm.Team)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found.");
        }

        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserDto>> CreateUserAsync(string adminUserId, CreateUserDto dto)
    {
        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return Result<UserDto>.Failure("This email address is already in use.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsAdmin = dto.IsAdmin,
            IsActive = true,
            EmailConfirmed = true, // Admin-created users don't need email confirmation
            CreatedByUserId = adminUserId
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<UserDto>.Failure(errors);
        }

        return await GetByIdAsync(user.Id);
    }

    public async Task<Result<UserDto>> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result<UserDto>.Failure("User not found.");
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.AvatarUrl = dto.AvatarUrl;
        user.IsActive = dto.IsActive;
        user.IsAdmin = dto.IsAdmin;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(userId);
    }

    public async Task<Result> DeactivateUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found.");
        }

        // Don't allow deactivating the last admin
        if (user.IsAdmin)
        {
            var adminCount = await _context.Users.CountAsync(u => u.IsAdmin && u.IsActive);
            if (adminCount <= 1)
            {
                return Result.Failure("The last administrator cannot be deactivated.");
            }
        }

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ReactivateUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found.");
        }

        user.IsActive = true;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
        {
            return Result.Failure("User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(errors);
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
        {
            return Result.Failure("New passwords do not match.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(errors);
        }

        return Result.Success();
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.IsAdmin ?? false;
    }

    public async Task<Result> AddUserToTeamAsync(string userId, Guid teamId, string role)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found.");
        }

        var team = await _context.Teams.FindAsync(teamId);
        if (team == null)
        {
            return Result.Failure("Team not found.");
        }

        // Check if already a member
        var existingMember = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);

        if (existingMember != null)
        {
            if (existingMember.IsActive)
            {
                return Result.Failure("User is already a member of this team.");
            }
            // Reactivate membership
            existingMember.IsActive = true;
            existingMember.Role = Enum.Parse<TeamRole>(role);
            existingMember.JoinedAt = DateTime.UtcNow;
        }
        else
        {
            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                Role = Enum.Parse<TeamRole>(role),
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.TeamMembers.Add(teamMember);
        }

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> RemoveUserFromTeamAsync(string userId, Guid teamId)
    {
        var membership = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);

        if (membership == null)
        {
            return Result.Failure("Membership not found.");
        }

        membership.IsActive = false;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private static UserDto MapToDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Teams = user.TeamMemberships
                .Where(tm => tm.IsActive)
                .Select(tm => new UserTeamDto
                {
                    TeamId = tm.TeamId,
                    TeamName = tm.Team?.Name ?? string.Empty,
                    TeamSlug = tm.Team?.Slug ?? string.Empty,
                    Role = tm.Role.ToString(),
                    JoinedAt = tm.JoinedAt ?? DateTime.UtcNow
                }).ToList()
        };
    }
}

