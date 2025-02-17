using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviders_Subscriptions_SubscriptionId",
                table: "ServiceProviders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviders_SubscriptionId",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "ServiceProviders");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Subscriptions",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Subscriptions",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ServiceProviderId",
                table: "Subscriptions",
                column: "ServiceProviderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_ServiceProviders_ServiceProviderId",
                table: "Subscriptions",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_ServiceProviders_ServiceProviderId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_ServiceProviderId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "ServiceProviders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_SubscriptionId",
                table: "ServiceProviders",
                column: "SubscriptionId",
                unique: true,
                filter: "[SubscriptionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviders_Subscriptions_SubscriptionId",
                table: "ServiceProviders",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
