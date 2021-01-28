using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionRequirements_Positions_PositionID",
                table: "PositionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionRequirements_Projects_ProjectID",
                table: "PositionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillRequirements_PositionRequirements_PositionRequirementID",
                table: "SkillRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillRequirements_Skills_SkillID",
                table: "SkillRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillRequirements",
                table: "SkillRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PositionRequirements",
                table: "PositionRequirements");

            migrationBuilder.RenameTable(
                name: "SkillRequirements",
                newName: "RequiredSkills");

            migrationBuilder.RenameTable(
                name: "PositionRequirements",
                newName: "RequiredPositions");

            migrationBuilder.RenameIndex(
                name: "IX_SkillRequirements_SkillID",
                table: "RequiredSkills",
                newName: "IX_RequiredSkills_SkillID");

            migrationBuilder.RenameIndex(
                name: "IX_SkillRequirements_PositionRequirementID",
                table: "RequiredSkills",
                newName: "IX_RequiredSkills_PositionRequirementID");

            migrationBuilder.RenameIndex(
                name: "IX_PositionRequirements_ProjectID",
                table: "RequiredPositions",
                newName: "IX_RequiredPositions_ProjectID");

            migrationBuilder.RenameIndex(
                name: "IX_PositionRequirements_PositionID",
                table: "RequiredPositions",
                newName: "IX_RequiredPositions_PositionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequiredSkills",
                table: "RequiredSkills",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequiredPositions",
                table: "RequiredPositions",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "861655ae-77e7-4b4d-b71a-96c56b57961b", new DateTime(2021, 1, 28, 20, 22, 58, 930, DateTimeKind.Local).AddTicks(2610), "AQAAAAEAACcQAAAAEPDYQgaszF9KsZEMfQyftpLSK7VdKM29A/Bgx4WqvU1Y8YKCFBbQvFo0908v0eWK5w==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "78bb4861-26ba-40d9-90a5-c6d0cbb9bed9");

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredPositions_Positions_PositionID",
                table: "RequiredPositions",
                column: "PositionID",
                principalTable: "Positions",
                principalColumn: "PosID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredPositions_Projects_ProjectID",
                table: "RequiredPositions",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredSkills_RequiredPositions_PositionRequirementID",
                table: "RequiredSkills",
                column: "PositionRequirementID",
                principalTable: "RequiredPositions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredSkills_Skills_SkillID",
                table: "RequiredSkills",
                column: "SkillID",
                principalTable: "Skills",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequiredPositions_Positions_PositionID",
                table: "RequiredPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequiredPositions_Projects_ProjectID",
                table: "RequiredPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequiredSkills_RequiredPositions_PositionRequirementID",
                table: "RequiredSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_RequiredSkills_Skills_SkillID",
                table: "RequiredSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequiredSkills",
                table: "RequiredSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequiredPositions",
                table: "RequiredPositions");

            migrationBuilder.RenameTable(
                name: "RequiredSkills",
                newName: "SkillRequirements");

            migrationBuilder.RenameTable(
                name: "RequiredPositions",
                newName: "PositionRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_RequiredSkills_SkillID",
                table: "SkillRequirements",
                newName: "IX_SkillRequirements_SkillID");

            migrationBuilder.RenameIndex(
                name: "IX_RequiredSkills_PositionRequirementID",
                table: "SkillRequirements",
                newName: "IX_SkillRequirements_PositionRequirementID");

            migrationBuilder.RenameIndex(
                name: "IX_RequiredPositions_ProjectID",
                table: "PositionRequirements",
                newName: "IX_PositionRequirements_ProjectID");

            migrationBuilder.RenameIndex(
                name: "IX_RequiredPositions_PositionID",
                table: "PositionRequirements",
                newName: "IX_PositionRequirements_PositionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillRequirements",
                table: "SkillRequirements",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PositionRequirements",
                table: "PositionRequirements",
                column: "ID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PositionRequirements_Positions_PositionID",
                table: "PositionRequirements",
                column: "PositionID",
                principalTable: "Positions",
                principalColumn: "PosID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionRequirements_Projects_ProjectID",
                table: "PositionRequirements",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillRequirements_PositionRequirements_PositionRequirementID",
                table: "SkillRequirements",
                column: "PositionRequirementID",
                principalTable: "PositionRequirements",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillRequirements_Skills_SkillID",
                table: "SkillRequirements",
                column: "SkillID",
                principalTable: "Skills",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
