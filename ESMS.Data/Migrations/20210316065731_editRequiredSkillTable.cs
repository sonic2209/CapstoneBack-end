using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editRequiredSkillTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificationID",
                table: "RequiredSkills");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "RequiredSkills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Exp",
                table: "RequiredSkills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CertificationLevel",
                table: "RequiredSkills",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificationLevel",
                table: "RequiredSkills");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Exp",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CertificationID",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
