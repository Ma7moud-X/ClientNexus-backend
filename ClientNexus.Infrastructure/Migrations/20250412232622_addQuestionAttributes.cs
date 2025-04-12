using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addQuestionAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Visibility",
                schema: "ClientNexusSchema",
                table: "Questions",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AnswerBody",
                schema: "ClientNexusSchema",
                table: "Questions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AnsweredAt",
                schema: "ClientNexusSchema",
                table: "Questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnswerHelpful",
                schema: "ClientNexusSchema",
                table: "Questions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionBody",
                schema: "ClientNexusSchema",
                table: "Questions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerBody",
                schema: "ClientNexusSchema",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnsweredAt",
                schema: "ClientNexusSchema",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsAnswerHelpful",
                schema: "ClientNexusSchema",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionBody",
                schema: "ClientNexusSchema",
                table: "Questions");

            migrationBuilder.AlterColumn<bool>(
                name: "Visibility",
                schema: "ClientNexusSchema",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);
        }
    }
}
