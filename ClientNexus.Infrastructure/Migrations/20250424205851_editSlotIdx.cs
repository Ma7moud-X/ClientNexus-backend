using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editSlotIdx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Slots_ServiceProviderId_Date",
                schema: "ClientNexusSchema",
                table: "Slots");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ServiceProviderId_Date_SlotType",
                schema: "ClientNexusSchema",
                table: "Slots",
                columns: new[] { "ServiceProviderId", "Date", "SlotType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Slots_ServiceProviderId_Date_SlotType",
                schema: "ClientNexusSchema",
                table: "Slots");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ServiceProviderId_Date",
                schema: "ClientNexusSchema",
                table: "Slots",
                columns: new[] { "ServiceProviderId", "Date" },
                unique: true);
        }
    }
}
