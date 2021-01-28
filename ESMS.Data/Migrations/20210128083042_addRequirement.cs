using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PositionRequirements",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionRequirements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PositionRequirements_Positions_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Positions",
                        principalColumn: "PosID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionRequirements_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillInPositions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SkillRequirements",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    PositionRequirementID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillRequirements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SkillRequirements_PositionRequirements_PositionRequirementID",
                        column: x => x.PositionRequirementID,
                        principalTable: "PositionRequirements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillRequirements_Skills_SkillID",
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
                values: new object[] { "5907b6e1-c035-4bf8-9919-5e14f9dd1cda", new DateTime(2021, 1, 28, 15, 30, 41, 512, DateTimeKind.Local).AddTicks(3717), "AQAAAAEAACcQAAAAEIR66WvHHaDcj4uilGrkoirRoG/AOY+G8P1ayJxme3EL97IR+44MZukoXPEMbmximw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "e614403e-6de5-448a-8671-a2e9c70d3d98");

            migrationBuilder.CreateIndex(
                name: "IX_PositionRequirements_PositionID",
                table: "PositionRequirements",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_PositionRequirements_ProjectID",
                table: "PositionRequirements",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillInPositions_PositionID",
                table: "SkillInPositions",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillInPositions_SkillID",
                table: "SkillInPositions",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirements_PositionRequirementID",
                table: "SkillRequirements",
                column: "PositionRequirementID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirements_SkillID",
                table: "SkillRequirements",
                column: "SkillID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillInPositions");

            migrationBuilder.DropTable(
                name: "SkillRequirements");

            migrationBuilder.DropTable(
                name: "PositionRequirements");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "6d0278b1-e6d1-4f14-86d5-93094f544493", new DateTime(2021, 1, 27, 12, 14, 19, 649, DateTimeKind.Local).AddTicks(7703), "AQAAAAEAACcQAAAAEELGL/ir9ABuXHNWEZymwZZcVSLqzq+AC711aG5tdOGuAR7scjDAMxfo966uJlvmbw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "9a5e54f7-0858-4434-9d55-86703b382450");
        }
    }
}
