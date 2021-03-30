using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Skills",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Positions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Certifications",
                type: "bit",
                nullable: false,
                defaultValue: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Certifications");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "fdc90d23-a0a3-4cbd-9698-bb3873992c4b", new DateTime(2021, 3, 29, 14, 35, 17, 691, DateTimeKind.Local).AddTicks(8298), "AQAAAAEAACcQAAAAEM0vpcgzqdIAq6ePZZcRLc7nH04GwRbLRL3ep79tKTuSOEvO49y6YL1plKvijzzTBA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "f35f3aa9-f2b1-4675-b02e-a27a23adc741");
        }
    }
}
