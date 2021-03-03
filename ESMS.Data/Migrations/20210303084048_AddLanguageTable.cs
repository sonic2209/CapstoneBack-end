using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class AddLanguageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LangID);
                });

            migrationBuilder.CreateTable(
                name: "EmpLanguages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LangID = table.Column<int>(type: "int", nullable: false),
                    LangLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLanguages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmpLanguages_Employees_EmpID",
                        column: x => x.EmpID,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLanguages_Languages_LangID",
                        column: x => x.LangID,
                        principalTable: "Languages",
                        principalColumn: "LangID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequiredLanguages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequiredPositionID = table.Column<int>(type: "int", nullable: false),
                    LangID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredLanguages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RequiredLanguages_Languages_LangID",
                        column: x => x.LangID,
                        principalTable: "Languages",
                        principalColumn: "LangID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequiredLanguages_RequiredPositions_RequiredPositionID",
                        column: x => x.RequiredPositionID,
                        principalTable: "RequiredPositions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "2fdcb336-75c3-4ff4-b6df-f53d33418360", new DateTime(2021, 3, 3, 15, 40, 48, 92, DateTimeKind.Local).AddTicks(8323), "AQAAAAEAACcQAAAAEFHmdBDiiLpMl1lbaCcg2/rcpLm4Dc/HB8x18CoMwAS1ojoQNNlrL9tzCYQy6f4+bA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "db685d75-288c-44b5-8dcd-7587d88b78d1");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLanguages_EmpID",
                table: "EmpLanguages",
                column: "EmpID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLanguages_LangID",
                table: "EmpLanguages",
                column: "LangID");

            migrationBuilder.CreateIndex(
                name: "IX_RequiredLanguages_LangID",
                table: "RequiredLanguages",
                column: "LangID");

            migrationBuilder.CreateIndex(
                name: "IX_RequiredLanguages_RequiredPositionID",
                table: "RequiredLanguages",
                column: "RequiredPositionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpLanguages");

            migrationBuilder.DropTable(
                name: "RequiredLanguages");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "ac046c9c-ea90-45dd-8498-31c885cbc000", new DateTime(2021, 2, 24, 15, 51, 59, 327, DateTimeKind.Local).AddTicks(8271), "AQAAAAEAACcQAAAAEFVBpXX5sAlg7ri9Gom6SuV/5u3ze0jlss7ZNu/i8rMZoFJKLeZbspEhm8e6ypzBRg==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "27169839-6a11-44ad-a080-e14d902d4420");
        }
    }
}
