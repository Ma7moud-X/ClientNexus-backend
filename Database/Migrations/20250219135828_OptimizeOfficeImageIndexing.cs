using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeOfficeImageIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OfficeImageUrls",
                table: "OfficeImageUrls");

            migrationBuilder.DropIndex(
                name: "IX_OfficeImageUrls_ServiceProviderId",
                table: "OfficeImageUrls");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfficeImageUrls",
                table: "OfficeImageUrls",
                columns: new[] { "ServiceProviderId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OfficeImageUrls",
                table: "OfficeImageUrls");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfficeImageUrls",
                table: "OfficeImageUrls",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeImageUrls_ServiceProviderId",
                table: "OfficeImageUrls",
                column: "ServiceProviderId");
        }
    }
}
