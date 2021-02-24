using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class editRequiredSkill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SkillLevel",
                table: "RequiredSkills",
                newName: "Exp");

            migrationBuilder.AddColumn<int>(
                name: "CertificationID",
                table: "RequiredSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificationID",
                table: "RequiredSkills");

            migrationBuilder.RenameColumn(
                name: "Exp",
                table: "RequiredSkills",
                newName: "SkillLevel");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "29b1fa1d-270e-48b1-af6d-9030922d64ed", new DateTime(2021, 2, 23, 15, 48, 10, 292, DateTimeKind.Local).AddTicks(1697), "AQAAAAEAACcQAAAAEEG45PuGhMt8kxFuB04vdULkMQ34LqquWXSwpi9YKNgBBlXAGjxt3wCpacNKdzzVsw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "614037bc-ccd0-450a-a4f2-94541b42eb54");
        }
    }
}
