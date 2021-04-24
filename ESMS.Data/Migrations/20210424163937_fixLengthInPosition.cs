using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class fixLengthInPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectFields_ProjectFieldID",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeID",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectTypeID",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectFieldID",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Positions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "894b71d0-786e-4aae-99bb-1d7f7c42f54e", new DateTime(2021, 4, 24, 23, 39, 36, 928, DateTimeKind.Local).AddTicks(6525), "AQAAAAEAACcQAAAAEJajMJaaw89l0KYbc/TNF1BKImPGwnRcQYTNw9nz4wiTlhQQSF3z987TrJFARbZ9pw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "886991b0-0913-4e62-9afd-8bc6f85372a9");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectFields_ProjectFieldID",
                table: "Projects",
                column: "ProjectFieldID",
                principalTable: "ProjectFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeID",
                table: "Projects",
                column: "ProjectTypeID",
                principalTable: "ProjectTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectFields_ProjectFieldID",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeID",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectTypeID",
                table: "Projects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectFieldID",
                table: "Projects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Positions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectFields_ProjectFieldID",
                table: "Projects",
                column: "ProjectFieldID",
                principalTable: "ProjectFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeID",
                table: "Projects",
                column: "ProjectTypeID",
                principalTable: "ProjectTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
