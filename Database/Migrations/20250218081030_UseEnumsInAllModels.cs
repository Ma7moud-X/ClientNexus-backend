using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UseEnumsInAllModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Slots",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldDefaultValue: "A");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldDefaultValue: "P");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Slots",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "A",
                oldClrType: typeof(string),
                oldType: "varchar(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "P",
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");
        }
    }
}
