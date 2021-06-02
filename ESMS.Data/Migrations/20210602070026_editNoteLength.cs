using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editNoteLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "EmpPositionInProjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "2beb31ef-d5c4-4d4d-bb5c-0c0a063915c1", new DateTime(2021, 6, 2, 14, 0, 25, 792, DateTimeKind.Local).AddTicks(9234), "AQAAAAEAACcQAAAAEG3j+W6TE5mRIEmy6SnvFRWwSHkeJiIxjR4jBs8Xg9Jokd1Na67O0oMyLW5YCr7wVw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "5044b20c-b8ce-4031-8e80-d4aa17341162");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "EmpPositionInProjects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "c449bd49-764f-4540-b024-f432a0433b1f", new DateTime(2021, 5, 31, 15, 39, 10, 332, DateTimeKind.Local).AddTicks(2373), "AQAAAAEAACcQAAAAEGD62o5pr8yeBo4HtFUvMgxViHikBcIr9z2h+FVFeDivIO4JgfuEpEnbvgpauKH/ag==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "1a38091b-e7e8-419d-af3e-26177a297378");
        }
    }
}
