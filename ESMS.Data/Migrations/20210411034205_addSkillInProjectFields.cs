using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addSkillInProjectFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MinPosInProjects",
                table: "MinPosInProjects");

            migrationBuilder.AddColumn<int>(
                name: "SkillID",
                table: "MinPosInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MinPosInProjects",
                table: "MinPosInProjects",
                columns: new[] { "TypeID", "PosID", "SkillID" });

            migrationBuilder.CreateTable(
                name: "SkillInProjectFields",
                columns: table => new
                {
                    FieldID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillInProjectFields", x => new { x.FieldID, x.SkillID });
                    table.ForeignKey(
                        name: "FK_SkillInProjectFields_ProjectFields_FieldID",
                        column: x => x.FieldID,
                        principalTable: "ProjectFields",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillInProjectFields_Skills_SkillID",
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
                values: new object[] { "b2dc3729-fb46-451a-9a9b-0c1a7e247855", new DateTime(2021, 4, 11, 10, 42, 4, 932, DateTimeKind.Local).AddTicks(1812), "AQAAAAEAACcQAAAAEIV/YlKm1z7yliSPfEQ4+uHugqHPZHzL5/ozrqIKDy9u0HABAluORjqjYavyH8HQHA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "90a0b22f-ee7a-461c-a8bf-ffea22cc399b");

            migrationBuilder.CreateIndex(
                name: "IX_MinPosInProjects_SkillID",
                table: "MinPosInProjects",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillInProjectFields_SkillID",
                table: "SkillInProjectFields",
                column: "SkillID");

            migrationBuilder.AddForeignKey(
                name: "FK_MinPosInProjects_Skills_SkillID",
                table: "MinPosInProjects",
                column: "SkillID",
                principalTable: "Skills",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinPosInProjects_Skills_SkillID",
                table: "MinPosInProjects");

            migrationBuilder.DropTable(
                name: "SkillInProjectFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MinPosInProjects",
                table: "MinPosInProjects");

            migrationBuilder.DropIndex(
                name: "IX_MinPosInProjects_SkillID",
                table: "MinPosInProjects");

            migrationBuilder.DropColumn(
                name: "SkillID",
                table: "MinPosInProjects");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MinPosInProjects",
                table: "MinPosInProjects",
                columns: new[] { "TypeID", "PosID" });

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
        }
    }
}
