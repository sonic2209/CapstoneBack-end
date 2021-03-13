using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class removeSkillInPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillInPositions");

            migrationBuilder.AlterColumn<int>(
                name: "SkillLevel",
                table: "EmpSkills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Exp",
                table: "EmpSkills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateEnd",
                table: "EmpSkills",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOut",
                table: "EmpPositions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTaken",
                table: "EmpCertifications",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateEnd",
                table: "EmpCertifications",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "04dac4f0-677b-407c-be6f-4cb14c7bede3", new DateTime(2021, 3, 10, 15, 49, 58, 923, DateTimeKind.Local).AddTicks(71), "AQAAAAEAACcQAAAAENxZAMGl3JbTg/ZaU1w0fiCnhZqgEuI0moTRAP/IoorVwd0m3PgNWoQnqOJixpHPCQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "526faabb-d76f-46a9-b370-46a81e0f01f5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SkillLevel",
                table: "EmpSkills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Exp",
                table: "EmpSkills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateEnd",
                table: "EmpSkills",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOut",
                table: "EmpPositions",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTaken",
                table: "EmpCertifications",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateEnd",
                table: "EmpCertifications",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SkillInPositions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillInPositions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SkillInPositions_Positions_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Positions",
                        principalColumn: "PosID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillInPositions_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_SkillInPositions_PositionID",
                table: "SkillInPositions",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillInPositions_SkillID",
                table: "SkillInPositions",
                column: "SkillID");
        }
    }
}
