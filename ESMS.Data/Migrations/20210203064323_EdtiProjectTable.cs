using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class EdtiProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateBegin",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEstimatedEnd",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "8ed7235e-423e-4f28-b431-360e733829e7", new DateTime(2021, 2, 3, 13, 43, 22, 787, DateTimeKind.Local).AddTicks(1672), "AQAAAAEAACcQAAAAEBL143TLIdhnGo4l0O/EDRtdhpt6y3Ca3tFYZkqsjl+qRoUpj28b2+zAjKvfslZptg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "088b5c3c-5817-4e19-a49c-5e33ad1d039b");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateBegin",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DateEstimatedEnd",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "d707bde1-88af-4857-9f67-e27d608be2c8", new DateTime(2021, 1, 31, 21, 51, 28, 916, DateTimeKind.Local).AddTicks(1578), "AQAAAAEAACcQAAAAEGVa638UezsuskV844g96Q2SnWZfQVpNcEqY4LtjNafHnBlQNbvtWqIoe/O4p9BZYQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "85bb8e8d-108e-4480-a50f-5b4e9889d37f");
        }
    }
}
