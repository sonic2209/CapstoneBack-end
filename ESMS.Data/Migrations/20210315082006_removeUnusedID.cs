using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class removeUnusedID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ID",
                table: "RequiredSkills");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "RequiredLanguages");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EmpSkills");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EmpPositions");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EmpLanguages");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EmpCertifications");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "4b0c9c25-2cec-4aaa-ab9c-eefe622817cb", new DateTime(2021, 3, 15, 15, 20, 5, 867, DateTimeKind.Local).AddTicks(4023), "AQAAAAEAACcQAAAAEMZ8m2589GOX4+voQ13OKJpzeppDm3bVNq46RktzAKQPTXWvt4JpfAW9U3tXJMDg7w==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "1644037c-95ac-4aa6-8e8c-cd9ce3019a02");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "RequiredLanguages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "EmpSkills",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "EmpPositions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "EmpLanguages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "EmpCertifications",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "fee8cbe0-dbdd-4810-833b-e371b34e6e47", new DateTime(2021, 3, 15, 15, 7, 13, 341, DateTimeKind.Local).AddTicks(4328), "AQAAAAEAACcQAAAAEELziwLgoBUSO8chl1YHDXikL9pcrR9FmZXB+fZMYrnMP4tJfCmch2RauJRzpCK2fw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "bad429df-6bef-496b-b6fa-b4ac462d3878");
        }
    }
}
