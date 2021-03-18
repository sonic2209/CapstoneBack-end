using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "bf5bdbc7-cf14-4e35-acef-5dfd48d29455", new DateTime(2021, 3, 18, 12, 28, 19, 61, DateTimeKind.Local).AddTicks(527), "AQAAAAEAACcQAAAAED5aZaPMqgb717aFzSKLVeohTatoKuHi+CMAEdKdvvgEustFyJI2eT3Sr1CZleUnkw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "db68debb-c9fc-4065-9751-f8bb0df3791c");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectName",
                table: "Projects",
                column: "ProjectName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectName",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "e3bac563-617f-4529-b179-25aa49fe97ab", new DateTime(2021, 3, 17, 16, 21, 7, 434, DateTimeKind.Local).AddTicks(9328), "AQAAAAEAACcQAAAAEJ55yoBwI/lC8e+CG1Gb06MocSK7qxoJebg8cNt9m8YFN786OWn53tb+d9b9o9qUpQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ae1abf48-f964-48e9-b862-f9871cbd6cf4");
        }
    }
}
