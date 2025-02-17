using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ModifyingProblemModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Problems",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReportedBy",
                table: "Problems",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Problems",
                type: "nvarchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ClientId",
                table: "Problems",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ServiceId",
                table: "Problems",
                column: "ServiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Services_ServiceId",
                table: "Problems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Services_ServiceId",
                table: "Problems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ClientId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ServiceId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Problems");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)");

            migrationBuilder.AlterColumn<string>(
                name: "ReportedBy",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                columns: new[] { "ClientId", "ServiceProviderId", "AdminId" });
        }
    }
}
