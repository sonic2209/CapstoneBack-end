using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addProjectField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.RenameColumn(
                name: "PositionLevel",
                table: "RequiredPositions",
                newName: "CandidateNeeded");

            migrationBuilder.AddColumn<int>(
                name: "ProjectFieldID",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectFields",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFields", x => x.ID);
                });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "be333124-ae12-4b40-900a-13ebc3941c74", new DateTime(2021, 4, 10, 13, 12, 31, 178, DateTimeKind.Local).AddTicks(2789), "AQAAAAEAACcQAAAAEEqIjMybMUi/yVfw0JnhWa0nJHmvx/GDUepeovHkf5ow92sx9Pp8WwvZe8JgPKz4Rw==" });

            migrationBuilder.InsertData(
                table: "ProjectFields",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Community and social services" },
                    { 2, "Business, management and administration" },
                    { 3, "Education" },
                    { 4, "Health and medicine" },
                    { 5, "Sales" }
                });

            migrationBuilder.UpdateData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "Name",
                value: "Web application");

            migrationBuilder.UpdateData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "Name",
                value: "Mobile application");

            migrationBuilder.UpdateData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "Name",
                value: "Desktop application");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "72de8ff1-e673-44c9-899f-cc14b91c0d38");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectFieldID",
                table: "Projects",
                column: "ProjectFieldID");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectFields_ProjectFieldID",
                table: "Projects",
                column: "ProjectFieldID",
                principalTable: "ProjectFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectFields_ProjectFieldID",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectFields");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectFieldID",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectFieldID",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "CandidateNeeded",
                table: "RequiredPositions",
                newName: "PositionLevel");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "d9d970c1-7e72-4567-b2e0-7514818602b8", new DateTime(2021, 4, 7, 16, 58, 22, 570, DateTimeKind.Local).AddTicks(560), "AQAAAAEAACcQAAAAEPfFQTLuOIH02TuKV48vJ2rdV98Cb9tuj/gUzqFWi5iybqRlT68vwIVbi81cn0Y0lQ==" });

            migrationBuilder.UpdateData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "Name",
                value: "Community and social services");

            migrationBuilder.UpdateData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "Name",
                value: "Business, management and administration");

            migrationBuilder.UpdateData(
                table: "ProjectTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "Name",
                value: "Education");

            migrationBuilder.InsertData(
                table: "ProjectTypes",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 4, "Health and medicine" },
                    { 5, "Sales" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "d1747b55-757a-4703-851c-baf5acbe562c");
        }
    }
}
