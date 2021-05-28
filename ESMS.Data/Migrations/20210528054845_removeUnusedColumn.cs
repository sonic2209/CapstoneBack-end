using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class removeUnusedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "02a61fc8-c230-40d5-8b77-2a0592c57847", new DateTime(2021, 5, 28, 12, 48, 45, 83, DateTimeKind.Local).AddTicks(6450), "AQAAAAEAACcQAAAAENeliJys13+n+dySMHMnkD6IXxvZn5tRv6sXCTqJ4Dk/ArjZLMNEaCBpyHJtJ05c8A==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "8cffd37f-7553-4718-992b-d216ae16c66f");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NormalizedEmail",
                table: "Employees",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NormalizedUserName",
                table: "Employees",
                column: "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_NormalizedEmail",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_NormalizedUserName",
                table: "Employees");

            migrationBuilder.AddColumn<bool>(
                name: "EmailStatus",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "ddd08e58-4634-40b0-8619-d0a4c2b78211", new DateTime(2021, 5, 25, 20, 35, 39, 770, DateTimeKind.Local).AddTicks(274), "AQAAAAEAACcQAAAAEEx6wL9WKoheY77c33lGhvgF5YLZLwRU2ZQYVD+Slm96fBPbMdOFOJS7rp8Bo6Zu7g==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "e443f298-abcd-4afb-8be1-f3ba5cabd2a6");
        }
    }
}
