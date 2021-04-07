using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editEmpPositionInProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailStatus",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOut",
                table: "EmpPositionInProjects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "d9d970c1-7e72-4567-b2e0-7514818602b8", new DateTime(2021, 4, 7, 16, 58, 22, 570, DateTimeKind.Local).AddTicks(560), "AQAAAAEAACcQAAAAEPfFQTLuOIH02TuKV48vJ2rdV98Cb9tuj/gUzqFWi5iybqRlT68vwIVbi81cn0Y0lQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "d1747b55-757a-4703-851c-baf5acbe562c");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DateOut",
                table: "EmpPositionInProjects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "e3a26502-72b3-49e8-a3d3-62f951173968", new DateTime(2021, 3, 30, 19, 29, 19, 764, DateTimeKind.Local).AddTicks(4308), "AQAAAAEAACcQAAAAEGOL8waBKkEnvczVWN+dSqgU4OfK58hwOXTUmcvM2zGuGOrLWzhIYk/g7e6OpR+N2Q==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "81a4b4aa-5d37-4608-bae7-e68d7b6cfcb7");
        }
    }
}