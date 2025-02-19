using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class FeedbacksPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ClientServiceProviderFeedbacks_ServiceProviderId",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                table: "ClientServiceProviderFeedbacks",
                columns: new[] { "ServiceProviderId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceProviderFeedbacks_ClientId",
                table: "ClientServiceProviderFeedbacks",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ClientServiceProviderFeedbacks_ClientId",
                table: "ClientServiceProviderFeedbacks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientServiceProviderFeedbacks",
                table: "ClientServiceProviderFeedbacks",
                columns: new[] { "ClientId", "ServiceProviderId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceProviderFeedbacks_ServiceProviderId",
                table: "ClientServiceProviderFeedbacks",
                column: "ServiceProviderId");
        }
    }
}
