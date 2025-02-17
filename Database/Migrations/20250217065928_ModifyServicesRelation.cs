using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ModifyServicesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ServiceProviders_ServiceProviderId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationCases_ServiceProviders_ServiceProviderId",
                table: "ConsultationCases");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_ServiceProviders_ServiceProviderId",
                table: "EmergencyCases");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ServiceProviders_ServiceProviderId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ServiceProviderId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_ServiceProviderId",
                table: "EmergencyCases");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationCases_ServiceProviderId",
                table: "ConsultationCases");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ServiceProviderId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "EmergencyCases");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "ConsultationCases");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "P",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Services",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceProviderId",
                table: "Services",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceProviders_ServiceProviderId",
                table: "Services",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceProviders_ServiceProviderId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceProviderId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Services");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldDefaultValue: "P");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Services",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "EmergencyCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "ConsultationCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ServiceProviderId",
                table: "Questions",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_ServiceProviderId",
                table: "EmergencyCases",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationCases_ServiceProviderId",
                table: "ConsultationCases",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceProviderId",
                table: "Appointments",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ServiceProviders_ServiceProviderId",
                table: "Appointments",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationCases_ServiceProviders_ServiceProviderId",
                table: "ConsultationCases",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_ServiceProviders_ServiceProviderId",
                table: "EmergencyCases",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ServiceProviders_ServiceProviderId",
                table: "Questions",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
