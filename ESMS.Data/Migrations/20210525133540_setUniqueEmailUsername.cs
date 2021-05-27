using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class setUniqueEmailUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserName",
                table: "Employees",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserName",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "ddc97e26-f0aa-4fba-894d-146cb2e73c69", new DateTime(2021, 5, 25, 15, 45, 49, 670, DateTimeKind.Local).AddTicks(9416), "AQAAAAEAACcQAAAAEHSOVic+bmOqlueCCWAVKoMFSDjHMHKYStwF8eVQ5mfVU9GINAIwMCwPlU6TXJcojg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "73523d9c-2dcf-455e-a6f4-77f6286f2f8f");
        }
    }
}
