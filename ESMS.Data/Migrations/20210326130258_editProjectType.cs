using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectType_ProjectTypeID",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectType",
                table: "ProjectType");

            migrationBuilder.RenameTable(
                name: "ProjectType",
                newName: "ProjectTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectTypes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTypes",
                table: "ProjectTypes",
                column: "ID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeID",
                table: "Projects",
                column: "ProjectTypeID",
                principalTable: "ProjectTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeID",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTypes",
                table: "ProjectTypes");

            migrationBuilder.RenameTable(
                name: "ProjectTypes",
                newName: "ProjectType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectType",
                table: "ProjectType",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "71abaf0e-7373-4e9d-823d-f5e4d80344c7", new DateTime(2021, 3, 26, 15, 31, 6, 427, DateTimeKind.Local).AddTicks(3821), "AQAAAAEAACcQAAAAEPTVqyeWSKlZ+GmgYOaPKWHDpwXHDyPAk7eFHsv/wZBupPqYpBzbN6BthHv1aNnU1g==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "b40d5690-6252-4c43-9504-78dbf1e44a5c");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectType_ProjectTypeID",
                table: "Projects",
                column: "ProjectTypeID",
                principalTable: "ProjectType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
