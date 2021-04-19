using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editEmpPositionInProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccept",
                table: "EmpPositionInProjects");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "30c7cb91-4b8f-48ce-9fba-8c2064ca7b8c", new DateTime(2021, 4, 19, 21, 56, 54, 939, DateTimeKind.Local).AddTicks(1582), "AQAAAAEAACcQAAAAEGQnch8o9vflu3IrXp+dWKZDDdlSWqp7sfxqyfHCMycezLGcRDKXm5c2u3wp60RSOA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "5d82c155-5607-40a5-94d0-fa113903bc81");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmpPositionInProjects");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccept",
                table: "EmpPositionInProjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "afcecd5e-9e6c-42dc-92b1-598e385c4a39", new DateTime(2021, 4, 16, 14, 21, 19, 713, DateTimeKind.Local).AddTicks(2471), "AQAAAAEAACcQAAAAEKB2lWmYFjH0BSCm2yUI12+AGkXVp9ZPglYXf7V8SDx/ZXu9efE4izZvv5NCoJXReA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "c232273d-5533-4c0d-b137-dccc009d2767");
        }
    }
}
