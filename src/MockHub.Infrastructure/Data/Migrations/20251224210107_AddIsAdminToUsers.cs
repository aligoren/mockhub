using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MockHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAdminToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "AspNetUsers");
        }
    }
}
