using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAppointAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentType",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationTime",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionTime",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderSentTime",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancellationTime",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CompletionTime",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReminderSent",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReminderSentTime",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "AppointmentType",
                schema: "ClientNexusSchema",
                table: "Appointments",
                type: "char(1)",
                nullable: false,
                defaultValue: "");
        }
    }
}
