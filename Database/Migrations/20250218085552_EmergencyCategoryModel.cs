using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class EmergencyCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "EmergencyCases");

            migrationBuilder.AddColumn<int>(
                name: "EmergencyCategoryId",
                table: "EmergencyCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmergencyCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceProviderTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyCategories_ServiceProviderTypes_ServiceProviderTypeId",
                        column: x => x.ServiceProviderTypeId,
                        principalTable: "ServiceProviderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_EmergencyCategoryId",
                table: "EmergencyCases",
                column: "EmergencyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCategories_ServiceProviderTypeId",
                table: "EmergencyCategories",
                column: "ServiceProviderTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_EmergencyCategories_EmergencyCategoryId",
                table: "EmergencyCases",
                column: "EmergencyCategoryId",
                principalTable: "EmergencyCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_EmergencyCategories_EmergencyCategoryId",
                table: "EmergencyCases");

            migrationBuilder.DropTable(
                name: "EmergencyCategories");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_EmergencyCategoryId",
                table: "EmergencyCases");

            migrationBuilder.DropColumn(
                name: "EmergencyCategoryId",
                table: "EmergencyCases");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "EmergencyCases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
