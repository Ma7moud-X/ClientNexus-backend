using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeServiceModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_EmergencyCategories_EmergencyCategoryId",
                schema: "ClientNexusSchema",
                table: "EmergencyCases");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_EmergencyCategoryId",
                schema: "ClientNexusSchema",
                table: "EmergencyCases");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                schema: "ClientNexusSchema",
                table: "EmergencyCases");

            migrationBuilder.DropColumn(
                name: "EmergencyCategoryId",
                schema: "ClientNexusSchema",
                table: "EmergencyCases");

            migrationBuilder.AlterColumn<int>(
                name: "TimeForArrival",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "MeetingLatitude",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MeetingLongitude",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingLatitude",
                schema: "ClientNexusSchema",
                table: "EmergencyCases");

            migrationBuilder.DropColumn(
                name: "MeetingLongitude",
                schema: "ClientNexusSchema",
                table: "EmergencyCases");

            migrationBuilder.AlterColumn<int>(
                name: "TimeForArrival",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EmergencyCategoryId",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_EmergencyCategoryId",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                column: "EmergencyCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_EmergencyCategories_EmergencyCategoryId",
                schema: "ClientNexusSchema",
                table: "EmergencyCases",
                column: "EmergencyCategoryId",
                principalSchema: "ClientNexusSchema",
                principalTable: "EmergencyCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
