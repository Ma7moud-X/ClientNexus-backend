using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeAppointRedundantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentProviderId",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Date",
                schema: "ClientNexusSchema",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentProviderId",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
