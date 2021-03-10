using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editEmpSkillDateStart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateStart",
                table: "EmpSkills",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateStart",
                table: "EmpSkills",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "04dac4f0-677b-407c-be6f-4cb14c7bede3", new DateTime(2021, 3, 10, 15, 49, 58, 923, DateTimeKind.Local).AddTicks(71), "AQAAAAEAACcQAAAAENxZAMGl3JbTg/ZaU1w0fiCnhZqgEuI0moTRAP/IoorVwd0m3PgNWoQnqOJixpHPCQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "526faabb-d76f-46a9-b370-46a81e0f01f5");
        }
    }
}
