using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class EmpPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpPositions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PosID = table.Column<int>(type: "int", nullable: false),
                    DateIn = table.Column<DateTime>(type: "date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "date", nullable: false),
                    NameExp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPositions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmpPositions_Employees_EmpID",
                        column: x => x.EmpID,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpPositions_Positions_PosID",
                        column: x => x.PosID,
                        principalTable: "Positions",
                        principalColumn: "PosID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "db2bdc7f-7e68-4dfa-b579-aa20607d7889", new DateTime(2021, 2, 22, 16, 11, 31, 819, DateTimeKind.Local).AddTicks(4457), "AQAAAAEAACcQAAAAEAlFjJSvUITZNMv0pqOh0JivHdj1R/lswlhIK8VmCHIQqmcl2DSNl/mLmXqX1KJ5Rw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "eacdaa7b-6aaa-40d9-8958-1f9c5cba947d");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositions_EmpID",
                table: "EmpPositions",
                column: "EmpID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositions_PosID",
                table: "EmpPositions",
                column: "PosID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpPositions");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "2a0b02cd-5e8b-4f3e-992b-259406b1df5c", new DateTime(2021, 2, 22, 15, 34, 7, 535, DateTimeKind.Local).AddTicks(3438), "AQAAAAEAACcQAAAAEMp62uzxlgqhvIRdEESIvZMxnjMlgrR5xtYIrQCw77lBoZBFGgZFAu9QSAwr3Q5jhw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "fb4c73f6-a0df-4c8e-b10b-a9c9db7fcd7a");
        }
    }
}
