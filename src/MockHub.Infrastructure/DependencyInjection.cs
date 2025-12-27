using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockHub.Application.Interfaces;
using MockHub.Domain.Entities;
using MockHub.Infrastructure.Data;
using MockHub.Infrastructure.Services;

namespace MockHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=mockhub.db";

        services.AddDbContext<MockHubDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<MockHubDbContext>()
        .AddDefaultTokenProviders();

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IMockProjectService, MockProjectService>();
        services.AddScoped<IMockEndpointService, MockEndpointService>();
        services.AddScoped<IMockResponseService, MockResponseService>();
        services.AddScoped<IRequestLogService, RequestLogService>();
        services.AddScoped<IImportService, MockHub.Application.Services.ImportService>();

        return services;
    }
}

