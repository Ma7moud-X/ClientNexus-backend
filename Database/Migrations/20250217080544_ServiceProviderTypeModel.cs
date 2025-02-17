using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ServiceProviderTypeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ServiceProviders",
                newName: "TypeId");

            migrationBuilder.CreateTable(
                name: "ServiceProviderTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ServiceProviderTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { -1, "Lawyer" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_TypeId",
                table: "ServiceProviders",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviders_ServiceProviderTypes_TypeId",
                table: "ServiceProviders",
                column: "TypeId",
                principalTable: "ServiceProviderTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviders_ServiceProviderTypes_TypeId",
                table: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "ServiceProviderTypes");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviders_TypeId",
                table: "ServiceProviders");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "ServiceProviders",
                newName: "Type");
        }
    }
}
