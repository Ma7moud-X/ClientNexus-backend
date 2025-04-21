using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProblemTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                schema: "ClientNexusSchema",
                table: "Problems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "ClientNexusSchema",
                table: "Problems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                schema: "ClientNexusSchema",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "ClientNexusSchema",
                table: "Problems");
        }
    }
}
