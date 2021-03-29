using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editEmpPositionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameExp",
                table: "EmpPositions",
                newName: "PositionLevel");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PositionLevel",
                table: "EmpPositions",
                newName: "NameExp");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "dad42770-2fe7-4a89-9a86-b3c47b51cf7a", new DateTime(2021, 3, 26, 20, 2, 58, 186, DateTimeKind.Local).AddTicks(3857), "AQAAAAEAACcQAAAAEFEOrYMTNB4PeIdp1MeB7/KRQHHjz1ruH8z1DStGn3yos+L2X3Jz/uruClYsT8BkzQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "7917cb6d-4ab5-4557-991b-233efb1a2dab");
        }
    }
}
