using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockHub.Domain.Entities;

namespace MockHub.Infrastructure.Data;

public class MockHubDbContext : IdentityDbContext<ApplicationUser>
{
    public MockHubDbContext(DbContextOptions<MockHubDbContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<MockProject> MockProjects => Set<MockProject>();
    public DbSet<MockEndpoint> MockEndpoints => Set<MockEndpoint>();
    public DbSet<MockResponse> MockResponses => Set<MockResponse>();
    public DbSet<ValidationRule> ValidationRules => Set<ValidationRule>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();
    public DbSet<DynamicVariable> DynamicVariables => Set<DynamicVariable>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(MockHubDbContext).Assembly);
    }
}


