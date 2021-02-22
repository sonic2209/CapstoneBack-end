using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editEmpSkill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "EmpSkills",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "EmpSkills",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Exp",
                table: "EmpSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "EmpSkills");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "EmpSkills");

            migrationBuilder.DropColumn(
                name: "Exp",
                table: "EmpSkills");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "c640bca4-3aad-4da2-8ca8-e828b028b848", new DateTime(2021, 2, 22, 16, 45, 16, 229, DateTimeKind.Local).AddTicks(1498), "AQAAAAEAACcQAAAAEJHj5etpQPDrdDcdSmzH0i+wg9feXOGHh6cuDonlLp9rHlm8tTbBNIaREMElILSR6A==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ede7fe5a-4084-4e30-9682-577005729cd8");
        }
    }
}
