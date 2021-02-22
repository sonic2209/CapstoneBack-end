using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class SkillCertification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SkillID",
                table: "Certifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_SkillID",
                table: "Certifications",
                column: "SkillID");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Skills_SkillID",
                table: "Certifications",
                column: "SkillID",
                principalTable: "Skills",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Skills_SkillID",
                table: "Certifications");

            migrationBuilder.DropIndex(
                name: "IX_Certifications_SkillID",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "SkillID",
                table: "Certifications");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "6442cf91-7670-4529-963a-25abd74ef351", new DateTime(2021, 2, 19, 22, 52, 30, 481, DateTimeKind.Local).AddTicks(8514), "AQAAAAEAACcQAAAAEOfQKyIVRdAFzBGPOQmpFjG4oS5qSWbbAVtwOF50YrYcirWoJn2pt9XZhjXsOAz21Q==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "6277496d-0a0a-42bc-9163-fc3f7123a4af");
        }
    }
}
