using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamingColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapLocation",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "ServiceProviders",
                type: "nvarchar(500)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "MapLocation",
                table: "ServiceProviders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
