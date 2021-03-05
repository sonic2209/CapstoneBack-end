using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addCertiLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertiLevel",
                table: "Certifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "15e5ccd4-6c39-4fae-a8cb-d36fe2fd711c", new DateTime(2021, 3, 5, 22, 40, 11, 526, DateTimeKind.Local).AddTicks(5802), "AQAAAAEAACcQAAAAEJI9keRdNiYMnz7ncj42XMZbF6FkAcrTXHEFrgO+sxMjnLIkaZjsgJ6SYCTEWIVYrg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "4676bd7e-457f-4cad-8642-56670c29c921");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertiLevel",
                table: "Certifications");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "2fdcb336-75c3-4ff4-b6df-f53d33418360", new DateTime(2021, 3, 3, 15, 40, 48, 92, DateTimeKind.Local).AddTicks(8323), "AQAAAAEAACcQAAAAEFHmdBDiiLpMl1lbaCcg2/rcpLm4Dc/HB8x18CoMwAS1ojoQNNlrL9tzCYQy6f4+bA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "db685d75-288c-44b5-8dcd-7587d88b78d1");
        }
    }
}
