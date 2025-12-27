using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MockProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    DefaultDelay = table.Column<int>(type: "INTEGER", nullable: false),
                    EnableCors = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableLogging = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableLatencySimulation = table.Column<bool>(type: "INTEGER", nullable: false),
                    GlobalLatencyMin = table.Column<int>(type: "INTEGER", nullable: true),
                    GlobalLatencyMax = table.Column<int>(type: "INTEGER", nullable: true),
                    EnableJwtValidation = table.Column<bool>(type: "INTEGER", nullable: false),
                    JwtSecret = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    JwtIssuer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    JwtAudience = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockProjects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MockProjects_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Expression = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPersistent = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurrentValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicVariables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicVariables_MockProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "MockProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MockEndpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Route = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsWildcard = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegexPattern = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ResponseMode = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentResponseIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    DelayMin = table.Column<int>(type: "INTEGER", nullable: true),
                    DelayMax = table.Column<int>(type: "INTEGER", nullable: true),
                    CallbackUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EnableCallback = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockEndpoints_MockProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "MockProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MockResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Headers = table.Column<string>(type: "TEXT", nullable: true),
                    Condition = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ConditionExpression = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsFileResponse = table.Column<bool>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockResponses_MockEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "MockEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Method = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    QueryString = table.Column<string>(type: "TEXT", nullable: true),
                    RequestHeaders = table.Column<string>(type: "TEXT", nullable: true),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseBody = table.Column<string>(type: "TEXT", nullable: true),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    ClientIp = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsMatched = table.Column<bool>(type: "INTEGER", nullable: false),
                    MatchedRoute = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestLogs_MockEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "MockEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RequestLogs_MockProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "MockProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParameterName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    RegexPattern = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MinValue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MaxValue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationRules_MockEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "MockEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DynamicVariables_ProjectId_Name",
                table: "DynamicVariables",
                columns: new[] { "ProjectId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockEndpoints_Order",
                table: "MockEndpoints",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_MockEndpoints_ProjectId_Route_Method",
                table: "MockEndpoints",
                columns: new[] { "ProjectId", "Route", "Method" });

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_Name",
                table: "MockProjects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_Port",
                table: "MockProjects",
                column: "Port",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_TeamId",
                table: "MockProjects",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_UserId",
                table: "MockProjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MockResponses_EndpointId_Order",
                table: "MockResponses",
                columns: new[] { "EndpointId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_MockResponses_IsDefault",
                table: "MockResponses",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_CreatedAt",
                table: "RequestLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_EndpointId",
                table: "RequestLogs",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_ProjectId",
                table: "RequestLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_ProjectId_CreatedAt",
                table: "RequestLogs",
                columns: new[] { "ProjectId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId_UserId",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationRules_EndpointId_ParameterName_Location",
                table: "ValidationRules",
                columns: new[] { "EndpointId", "ParameterName", "Location" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DynamicVariables");

            migrationBuilder.DropTable(
                name: "MockResponses");

            migrationBuilder.DropTable(
                name: "RequestLogs");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "ValidationRules");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "MockEndpoints");

            migrationBuilder.DropTable(
                name: "MockProjects");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
