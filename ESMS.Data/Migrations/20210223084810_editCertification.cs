using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editCertification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Certifications");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "29b1fa1d-270e-48b1-af6d-9030922d64ed", new DateTime(2021, 2, 23, 15, 48, 10, 292, DateTimeKind.Local).AddTicks(1697), "AQAAAAEAACcQAAAAEEG45PuGhMt8kxFuB04vdULkMQ34LqquWXSwpi9YKNgBBlXAGjxt3wCpacNKdzzVsw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "614037bc-ccd0-450a-a4f2-94541b42eb54");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Certifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "d135558e-2532-4d8b-8bad-ef5a9393116b", new DateTime(2021, 2, 22, 22, 33, 53, 67, DateTimeKind.Local).AddTicks(2981), "AQAAAAEAACcQAAAAEJPL5mVWBQxCnereNMYtqdpqJWWa0BZvB2zeukRGbUzOsMHk7yeHRlgZyIjZ2S2eIg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "b294fee3-6066-4244-b278-b973997c19a1");
        }
    }
}
