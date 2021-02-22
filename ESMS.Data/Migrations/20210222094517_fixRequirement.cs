using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class fixRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SkillLevel",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCandidates",
                table: "RequiredPositions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SkillLevel",
                table: "EmpSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "RequiredSkills");

            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "RequiredSkills");

            migrationBuilder.DropColumn(
                name: "NumberOfCandidates",
                table: "RequiredPositions");

            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "EmpSkills");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "db2bdc7f-7e68-4dfa-b579-aa20607d7889", new DateTime(2021, 2, 22, 16, 11, 31, 819, DateTimeKind.Local).AddTicks(4457), "AQAAAAEAACcQAAAAEAlFjJSvUITZNMv0pqOh0JivHdj1R/lswlhIK8VmCHIQqmcl2DSNl/mLmXqX1KJ5Rw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "eacdaa7b-6aaa-40d9-8958-1f9c5cba947d");
        }
    }
}
