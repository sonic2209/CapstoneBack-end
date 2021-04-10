using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addMinPosInProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MinPosInProjects",
                columns: table => new
                {
                    TypeID = table.Column<int>(type: "int", nullable: false),
                    PosID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinPosInProjects", x => new { x.TypeID, x.PosID });
                    table.ForeignKey(
                        name: "FK_MinPosInProjects_Positions_PosID",
                        column: x => x.PosID,
                        principalTable: "Positions",
                        principalColumn: "PosID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinPosInProjects_ProjectTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "ProjectTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "31704fe3-6194-4926-8dfe-1bbe74539d1a", new DateTime(2021, 4, 10, 18, 30, 6, 780, DateTimeKind.Local).AddTicks(726), "AQAAAAEAACcQAAAAEBnLDdFSv7Exue0NP4RCyNaokMMaWzOAKgY9PG1eCQqem/n4iNM+nLDXcN3yaYhK4Q==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "a88696bc-f26d-4cf7-bbae-0916ac7e080d");

            migrationBuilder.CreateIndex(
                name: "IX_MinPosInProjects_PosID",
                table: "MinPosInProjects",
                column: "PosID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinPosInProjects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "99b5ac78-df04-4108-9cc1-8836b0f6ba9e", new DateTime(2021, 4, 10, 16, 4, 41, 635, DateTimeKind.Local).AddTicks(1994), "AQAAAAEAACcQAAAAEBEUssk3v6T/k25nIGawc579y3YTlYK05HAQMh32U3+RH7MpCvxamyB3mRI9FqKF3w==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "ff758830-1f36-4ce1-bbd6-da07c0987973");
        }
    }
}
