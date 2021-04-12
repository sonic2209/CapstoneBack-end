using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editRequiredPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RequiredPositions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MissingEmployee",
                table: "RequiredPositions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "RequiredPositions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "ca4e31d6-99dc-4008-86e5-d43c9c9de802", new DateTime(2021, 4, 12, 16, 26, 42, 873, DateTimeKind.Local).AddTicks(7704), "AQAAAAEAACcQAAAAEN+YGFPEk8tep28NLbYVNZ2PW/RmMHd1av6akkRikVT03uYeLtfvtm8sWfyl+Gn7pA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "4a4c3392-c512-49f2-ba05-9c3569cbd75f");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RequiredPositions");

            migrationBuilder.DropColumn(
                name: "MissingEmployee",
                table: "RequiredPositions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RequiredPositions");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "b2dc3729-fb46-451a-9a9b-0c1a7e247855", new DateTime(2021, 4, 11, 10, 42, 4, 932, DateTimeKind.Local).AddTicks(1812), "AQAAAAEAACcQAAAAEIV/YlKm1z7yliSPfEQ4+uHugqHPZHzL5/ozrqIKDy9u0HABAluORjqjYavyH8HQHA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "90a0b22f-ee7a-461c-a8bf-ffea22cc399b");
        }
    }
}
