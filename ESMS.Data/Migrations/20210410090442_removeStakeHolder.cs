using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class removeStakeHolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Skateholder",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "99b5ac78-df04-4108-9cc1-8836b0f6ba9e", new DateTime(2021, 4, 10, 16, 4, 41, 635, DateTimeKind.Local).AddTicks(1994), "AQAAAAEAACcQAAAAEBEUssk3v6T/k25nIGawc579y3YTlYK05HAQMh32U3+RH7MpCvxamyB3mRI9FqKF3w==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ff758830-1f36-4ce1-bbd6-da07c0987973");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Skateholder",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "be333124-ae12-4b40-900a-13ebc3941c74", new DateTime(2021, 4, 10, 13, 12, 31, 178, DateTimeKind.Local).AddTicks(2789), "AQAAAAEAACcQAAAAEEqIjMybMUi/yVfw0JnhWa0nJHmvx/GDUepeovHkf5ow92sx9Pp8WwvZe8JgPKz4Rw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "72de8ff1-e673-44c9-899f-cc14b91c0d38");
        }
    }
}
