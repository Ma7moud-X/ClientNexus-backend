using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ColumnsRenaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentsCategories_Categories_CategoryId",
                table: "DocumentsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentsCategories_Documents_DocumentId",
                table: "DocumentsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Admins_AdminId",
                table: "Problems");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentsCategories",
                table: "DocumentsCategories");

            migrationBuilder.RenameTable(
                name: "DocumentsCategories",
                newName: "DocumentCategories");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Problems",
                newName: "SolvingAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_AdminId",
                table: "Problems",
                newName: "IX_Problems_SolvingAdminId");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "EmergencyCases",
                newName: "CurrentLocation");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "DocumentCategories",
                newName: "DCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentsCategories_CategoryId",
                table: "DocumentCategories",
                newName: "IX_DocumentCategories_DCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentCategories",
                table: "DocumentCategories",
                columns: new[] { "DocumentId", "DCategoryId" });

            migrationBuilder.CreateTable(
                name: "DCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DCategories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentCategories_DCategories_DCategoryId",
                table: "DocumentCategories",
                column: "DCategoryId",
                principalTable: "DCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentCategories_Documents_DocumentId",
                table: "DocumentCategories",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Admins_SolvingAdminId",
                table: "Problems",
                column: "SolvingAdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentCategories_DCategories_DCategoryId",
                table: "DocumentCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentCategories_Documents_DocumentId",
                table: "DocumentCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Admins_SolvingAdminId",
                table: "Problems");

            migrationBuilder.DropTable(
                name: "DCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentCategories",
                table: "DocumentCategories");

            migrationBuilder.RenameTable(
                name: "DocumentCategories",
                newName: "DocumentsCategories");

            migrationBuilder.RenameColumn(
                name: "SolvingAdminId",
                table: "Problems",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_SolvingAdminId",
                table: "Problems",
                newName: "IX_Problems_AdminId");

            migrationBuilder.RenameColumn(
                name: "CurrentLocation",
                table: "EmergencyCases",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "DCategoryId",
                table: "DocumentsCategories",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentCategories_DCategoryId",
                table: "DocumentsCategories",
                newName: "IX_DocumentsCategories_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentsCategories",
                table: "DocumentsCategories",
                columns: new[] { "DocumentId", "CategoryId" });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentsCategories_Categories_CategoryId",
                table: "DocumentsCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentsCategories_Documents_DocumentId",
                table: "DocumentsCategories",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Admins_AdminId",
                table: "Problems",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
