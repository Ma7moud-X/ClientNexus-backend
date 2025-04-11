using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceProviderFeedbacks_ServiceProviderId",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks",
                column: "ServiceProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ClientServiceProviderFeedbacks_ServiceProviderId",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks",
                columns: new[] { "ServiceProviderId", "ClientId" });
        }
    }
}
