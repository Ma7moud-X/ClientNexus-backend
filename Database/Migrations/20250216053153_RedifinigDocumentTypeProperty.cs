using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RedifinigDocumentTypeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Documents",
                type: "nvarchar(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DocumentTypeId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
