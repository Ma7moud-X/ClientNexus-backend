using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryKeysOptimization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Slots_SlotId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Slots",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_ServiceProviderId",
                table: "Slots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ServiceProviderId",
                table: "Problems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers");

            migrationBuilder.DropIndex(
                name: "IX_PhoneNumbers_BaseUserId",
                table: "PhoneNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Licenses",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_ServiceProviderId",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentProviderId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slots",
                table: "Slots",
                columns: new[] { "ServiceProviderId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                columns: new[] { "ServiceProviderId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers",
                columns: new[] { "BaseUserId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Licenses",
                table: "Licenses",
                columns: new[] { "ServiceProviderId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentProviderId_SlotId",
                table: "Appointments",
                columns: new[] { "AppointmentProviderId", "SlotId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Slots_AppointmentProviderId_SlotId",
                table: "Appointments",
                columns: new[] { "AppointmentProviderId", "SlotId" },
                principalTable: "Slots",
                principalColumns: new[] { "ServiceProviderId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Slots_AppointmentProviderId_SlotId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Slots",
                table: "Slots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Licenses",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentProviderId_SlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentProviderId",
                table: "Appointments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slots",
                table: "Slots",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Licenses",
                table: "Licenses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ServiceProviderId",
                table: "Slots",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ServiceProviderId",
                table: "Problems",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumbers_BaseUserId",
                table: "PhoneNumbers",
                column: "BaseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ServiceProviderId",
                table: "Licenses",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Slots_SlotId",
                table: "Appointments",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
