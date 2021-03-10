using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editRequiredLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "RequiredLanguages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "0fd51efd-5b8e-4ba5-bba7-653c055766bb", new DateTime(2021, 3, 10, 17, 49, 56, 810, DateTimeKind.Local).AddTicks(3457), "AQAAAAEAACcQAAAAEKbUs5O1jgXSbUU/I29Wodj4eQkI/Ie/Jqns/5LBR5ci9PQS/hWomvJHymCcFR1tKg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ec496a28-fb68-4ef1-9b39-0a2883eddec2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "RequiredLanguages");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "9e6d7c16-f34f-4dc7-9a73-a1f6ca415748", new DateTime(2021, 3, 10, 17, 11, 54, 308, DateTimeKind.Local).AddTicks(6976), "AQAAAAEAACcQAAAAEG+xsGUYwQNBLrg8OXjAwjH/1g2v/GfK9vAqcz9u2rqEEQpMgXKmTkVUYBqKhPyg7Q==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "9951e0aa-cf93-413e-bf10-09c893fc4715");
        }
    }
}
