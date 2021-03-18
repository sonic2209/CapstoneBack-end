using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpCertifications_Employees_EmpID",
                table: "EmpCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpLanguages_Employees_EmpID",
                table: "EmpLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_Employees_EmpID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositions_Employees_EmpID",
                table: "EmpPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpSkills_Employees_EmpID",
                table: "EmpSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequiredSkills",
                table: "RequiredSkills");

            migrationBuilder.DropIndex(
                name: "IX_RequiredSkills_SkillID",
                table: "RequiredSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequiredLanguages",
                table: "RequiredLanguages");

            migrationBuilder.DropIndex(
                name: "IX_RequiredLanguages_LangID",
                table: "RequiredLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpSkills",
                table: "EmpSkills");

            migrationBuilder.DropIndex(
                name: "IX_EmpSkills_EmpID",
                table: "EmpSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpPositions",
                table: "EmpPositions");

            migrationBuilder.DropIndex(
                name: "IX_EmpPositions_EmpID",
                table: "EmpPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmpPositionInProjects_ProjectID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpLanguages",
                table: "EmpLanguages");

            migrationBuilder.DropIndex(
                name: "IX_EmpLanguages_EmpID",
                table: "EmpLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpCertifications",
                table: "EmpCertifications");

            migrationBuilder.DropIndex(
                name: "IX_EmpCertifications_EmpID",
                table: "EmpCertifications");

            migrationBuilder.RenameColumn(
                name: "EmpSkillID",
                table: "EmpSkills",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpSkills",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpPositions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpPositionInProjects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateIn",
                table: "EmpPositionInProjects",
                type: "date",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpLanguages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpCertifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequiredSkills",
                table: "RequiredSkills",
                columns: new[] { "SkillID", "RequiredPositionID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequiredLanguages",
                table: "RequiredLanguages",
                columns: new[] { "LangID", "RequiredPositionID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpSkills",
                table: "EmpSkills",
                columns: new[] { "EmpID", "SkillID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpPositions",
                table: "EmpPositions",
                columns: new[] { "EmpID", "PosID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects",
                columns: new[] { "ProjectID", "PosID", "EmpID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpLanguages",
                table: "EmpLanguages",
                columns: new[] { "EmpID", "LangID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpCertifications",
                table: "EmpCertifications",
                columns: new[] { "EmpID", "CertificationID" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_EmpCertifications_Employees_EmpID",
                table: "EmpCertifications",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLanguages_Employees_EmpID",
                table: "EmpLanguages",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_Employees_EmpID",
                table: "EmpPositionInProjects",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositions_Employees_EmpID",
                table: "EmpPositions",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpSkills_Employees_EmpID",
                table: "EmpSkills",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpCertifications_Employees_EmpID",
                table: "EmpCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpLanguages_Employees_EmpID",
                table: "EmpLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_Employees_EmpID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositions_Employees_EmpID",
                table: "EmpPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpSkills_Employees_EmpID",
                table: "EmpSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequiredSkills",
                table: "RequiredSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequiredLanguages",
                table: "RequiredLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpSkills",
                table: "EmpSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpPositions",
                table: "EmpPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpLanguages",
                table: "EmpLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpCertifications",
                table: "EmpCertifications");

            migrationBuilder.DropColumn(
                name: "DateIn",
                table: "EmpPositionInProjects");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EmpSkills",
                newName: "EmpSkillID");

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpSkills",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpPositions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpPositionInProjects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpLanguages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmpID",
                table: "EmpCertifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequiredSkills",
                table: "RequiredSkills",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequiredLanguages",
                table: "RequiredLanguages",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpSkills",
                table: "EmpSkills",
                column: "EmpSkillID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpPositions",
                table: "EmpPositions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpLanguages",
                table: "EmpLanguages",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpCertifications",
                table: "EmpCertifications",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "0fd51efd-5b8e-4ba5-bba7-653c055766bb", new DateTime(2021, 3, 10, 17, 49, 56, 810, DateTimeKind.Local).AddTicks(3457), "AQAAAAEAACcQAAAAEKbUs5O1jgXSbUU/I29Wodj4eQkI/Ie/Jqns/5LBR5ci9PQS/hWomvJHymCcFR1tKg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ec496a28-fb68-4ef1-9b39-0a2883eddec2");

            migrationBuilder.CreateIndex(
                name: "IX_RequiredSkills_SkillID",
                table: "RequiredSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_RequiredLanguages_LangID",
                table: "RequiredLanguages",
                column: "LangID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpSkills_EmpID",
                table: "EmpSkills",
                column: "EmpID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositions_EmpID",
                table: "EmpPositions",
                column: "EmpID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositionInProjects_ProjectID",
                table: "EmpPositionInProjects",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLanguages_EmpID",
                table: "EmpLanguages",
                column: "EmpID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpCertifications_EmpID",
                table: "EmpCertifications",
                column: "EmpID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpCertifications_Employees_EmpID",
                table: "EmpCertifications",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLanguages_Employees_EmpID",
                table: "EmpLanguages",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_Employees_EmpID",
                table: "EmpPositionInProjects",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositions_Employees_EmpID",
                table: "EmpPositions",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpSkills_Employees_EmpID",
                table: "EmpSkills",
                column: "EmpID",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
