using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "ServiceProviderTypes",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "Countries",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 1, "EG", "مصر" });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "ServiceProviderTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Lawyer" },
                    { 2, "Doctor" }
                });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "Specializations",
                columns: new[] { "Id", "Name", "ServiceProviderTypeId" },
                values: new object[,]
                {
                    { 1, "جنائى", 1 },
                    { 2, "مدنى", 1 },
                    { 3, "اسرة", 1 }
                });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "States",
                columns: new[] { "Id", "Abbreviation", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, "CA", 1, "القاهرة" },
                    { 2, "GZ", 1, "الجيزة" },
                    { 3, "ALX", 1, "الاسكندرية" }
                });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "Cities",
                columns: new[] { "Id", "Abbreviation", "CountryId", "Name", "StateId" },
                values: new object[,]
                {
                    { 1, null, 1, "مدينة نصر", 1 },
                    { 2, null, 1, "الهرم", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "ServiceProviderTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "States",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "ServiceProviderTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "States",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "States",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "ClientNexusSchema",
                table: "Countries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "ServiceProviderTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { -1, "Lawyer" });
        }
    }
}
