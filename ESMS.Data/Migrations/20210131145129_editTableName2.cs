using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editTableName2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequiredSkills_RequiredPositions_PositionRequirementID",
                table: "RequiredSkills");

            migrationBuilder.RenameColumn(
                name: "PositionRequirementID",
                table: "RequiredSkills",
                newName: "RequiredPositionID");

            migrationBuilder.RenameIndex(
                name: "IX_RequiredSkills_PositionRequirementID",
                table: "RequiredSkills",
                newName: "IX_RequiredSkills_RequiredPositionID");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "d707bde1-88af-4857-9f67-e27d608be2c8", new DateTime(2021, 1, 31, 21, 51, 28, 916, DateTimeKind.Local).AddTicks(1578), "AQAAAAEAACcQAAAAEGVa638UezsuskV844g96Q2SnWZfQVpNcEqY4LtjNafHnBlQNbvtWqIoe/O4p9BZYQ==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "85bb8e8d-108e-4480-a50f-5b4e9889d37f");

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredSkills_RequiredPositions_RequiredPositionID",
                table: "RequiredSkills",
                column: "RequiredPositionID",
                principalTable: "RequiredPositions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequiredSkills_RequiredPositions_RequiredPositionID",
                table: "RequiredSkills");

            migrationBuilder.RenameColumn(
                name: "RequiredPositionID",
                table: "RequiredSkills",
                newName: "PositionRequirementID");

            migrationBuilder.RenameIndex(
                name: "IX_RequiredSkills_RequiredPositionID",
                table: "RequiredSkills",
                newName: "IX_RequiredSkills_PositionRequirementID");

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
                name: "FK_RequiredSkills_RequiredPositions_PositionRequirementID",
                table: "RequiredSkills",
                column: "PositionRequirementID",
                principalTable: "RequiredPositions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
