using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editRequiredTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exp",
                table: "EmpSkills");

            migrationBuilder.RenameColumn(
                name: "Exp",
                table: "RequiredSkills",
                newName: "SkillLevel");

            migrationBuilder.RenameColumn(
                name: "NumberOfCandidates",
                table: "RequiredPositions",
                newName: "PositionLevel");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "e3bac563-617f-4529-b179-25aa49fe97ab", new DateTime(2021, 3, 17, 16, 21, 7, 434, DateTimeKind.Local).AddTicks(9328), "AQAAAAEAACcQAAAAEJ55yoBwI/lC8e+CG1Gb06MocSK7qxoJebg8cNt9m8YFN786OWn53tb+d9b9o9qUpQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ae1abf48-f964-48e9-b862-f9871cbd6cf4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SkillLevel",
                table: "RequiredSkills",
                newName: "Exp");

            migrationBuilder.RenameColumn(
                name: "PositionLevel",
                table: "RequiredPositions",
                newName: "NumberOfCandidates");

            migrationBuilder.AddColumn<int>(
                name: "Exp",
                table: "EmpSkills",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "3b1d2c29-da0c-4c56-ab42-9a8db0fb0229", new DateTime(2021, 3, 16, 13, 57, 31, 142, DateTimeKind.Local).AddTicks(6075), "AQAAAAEAACcQAAAAEBQ3VvXau/BXqsXWSoiQoeESep90ONpV58+RKobBGQlIUU7zqogqR8qRucSustQYeQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "64a3c62c-03f9-4ad9-ab1d-e64039e30bcc");
        }
    }
}
