using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugRemovePort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MockProjects_Port",
                table: "MockProjects");

            migrationBuilder.DropIndex(
                name: "IX_MockProjects_TeamId",
                table: "MockProjects");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                table: "MockProjects");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "MockProjects");

            migrationBuilder.DropColumn(
                name: "ServerStatus",
                table: "MockProjects");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Teams",
                type: "TEXT",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "MockProjects",
                type: "TEXT",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Slug",
                table: "Teams",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_Slug",
                table: "MockProjects",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_TeamId_Slug",
                table: "MockProjects",
                columns: new[] { "TeamId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_Slug",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_MockProjects_Slug",
                table: "MockProjects");

            migrationBuilder.DropIndex(
                name: "IX_MockProjects_TeamId_Slug",
                table: "MockProjects");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "MockProjects");

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "MockProjects",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "MockProjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServerStatus",
                table: "MockProjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_Port",
                table: "MockProjects",
                column: "Port",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockProjects_TeamId",
                table: "MockProjects",
                column: "TeamId");
        }
    }
}
