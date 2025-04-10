using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeSlotPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Slots_AppointmentProviderId_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Slots",
                schema: "ClientNexusSchema",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentProviderId_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slots",
                schema: "ClientNexusSchema",
                table: "Slots",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ServiceProviderId_Date",
                schema: "ClientNexusSchema",
                table: "Slots",
                columns: new[] { "ServiceProviderId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Slots_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments",
                column: "SlotId",
                principalSchema: "ClientNexusSchema",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Slots_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Slots",
                schema: "ClientNexusSchema",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_ServiceProviderId_Date",
                schema: "ClientNexusSchema",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slots",
                schema: "ClientNexusSchema",
                table: "Slots",
                columns: new[] { "ServiceProviderId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentProviderId_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments",
                columns: new[] { "AppointmentProviderId", "SlotId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Slots_AppointmentProviderId_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments",
                columns: new[] { "AppointmentProviderId", "SlotId" },
                principalSchema: "ClientNexusSchema",
                principalTable: "Slots",
                principalColumns: new[] { "ServiceProviderId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
