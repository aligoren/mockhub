using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SidebarCollapsed",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ThemePreference",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SidebarCollapsed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ThemePreference",
                table: "AspNetUsers");
        }
    }
}
