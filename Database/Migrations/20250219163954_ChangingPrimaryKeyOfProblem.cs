using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangingPrimaryKeyOfProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ServiceProviderId",
                table: "Problems",
                column: "ServiceProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ServiceProviderId",
                table: "Problems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                columns: new[] { "ServiceProviderId", "Id" });
        }
    }
}
