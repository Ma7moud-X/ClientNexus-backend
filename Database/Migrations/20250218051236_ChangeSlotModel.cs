using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSlotModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SlotsServiceProviders");

            migrationBuilder.DropTable(
                name: "SlotTypes");

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Slots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SlotType",
                table: "Slots",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ServiceProviderId",
                table: "Slots",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_ServiceProviders_ServiceProviderId",
                table: "Slots",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_ServiceProviders_ServiceProviderId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_ServiceProviderId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SlotType",
                table: "Slots");

            migrationBuilder.CreateTable(
                name: "SlotsServiceProviders",
                columns: table => new
                {
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotsServiceProviders", x => new { x.SlotId, x.ServiceProviderId });
                    table.ForeignKey(
                        name: "FK_SlotsServiceProviders_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SlotsServiceProviders_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlotTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotTypes", x => new { x.Id, x.SlotId });
                    table.ForeignKey(
                        name: "FK_SlotTypes_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SlotsServiceProviders_ServiceProviderId",
                table: "SlotsServiceProviders",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotTypes_SlotId",
                table: "SlotTypes",
                column: "SlotId");
        }
    }
}
