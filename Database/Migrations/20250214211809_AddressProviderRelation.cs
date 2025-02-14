using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddressProviderRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviders_Addresses_AddressId",
                table: "ServiceProviders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviders_AddressId",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "Addresses",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ServiceProviderId",
                table: "Addresses",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_ServiceProviders_ServiceProviderId",
                table: "Addresses",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_ServiceProviders_ServiceProviderId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ServiceProviderId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "ServiceProviders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_AddressId",
                table: "ServiceProviders",
                column: "AddressId",
                unique: true,
                filter: "[AddressId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviders_Addresses_AddressId",
                table: "ServiceProviders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
