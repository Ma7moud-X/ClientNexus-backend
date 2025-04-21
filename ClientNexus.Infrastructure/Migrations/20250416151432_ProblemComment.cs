using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProblemComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                schema: "ClientNexusSchema",
                table: "Problems",
                type: "nvarchar(1000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComment",
                schema: "ClientNexusSchema",
                table: "Problems");
        }
    }
}
