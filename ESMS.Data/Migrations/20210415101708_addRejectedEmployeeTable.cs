using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addRejectedEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredPositionID",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RejectedEmployees",
                columns: table => new
                {
                    EmpID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequiredPositionID = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectedEmployees", x => new { x.EmpID, x.RequiredPositionID });
                    table.ForeignKey(
                        name: "FK_RejectedEmployees_Employees_EmpID",
                        column: x => x.EmpID,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RejectedEmployees_RequiredPositions_RequiredPositionID",
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
                values: new object[] { "34f036da-d938-4e70-912c-72de7930c55a", new DateTime(2021, 4, 15, 17, 17, 8, 60, DateTimeKind.Local).AddTicks(301), "AQAAAAEAACcQAAAAEGIKwGkZ+C6nno/7g9htnoUQokrB5f/xrPF6/nbh67Pw0h5c2pocx+iVHijI/CGdww==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "54a9cb1b-990d-420f-a934-72a3630291b2");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositionInProjects_RequiredPositionID",
                table: "EmpPositionInProjects",
                column: "RequiredPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_RejectedEmployees_RequiredPositionID",
                table: "RejectedEmployees",
                column: "RequiredPositionID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_RequiredPositions_RequiredPositionID",
                table: "EmpPositionInProjects",
                column: "RequiredPositionID",
                principalTable: "RequiredPositions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_RequiredPositions_RequiredPositionID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropTable(
                name: "RejectedEmployees");

            migrationBuilder.DropIndex(
                name: "IX_EmpPositionInProjects_RequiredPositionID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropColumn(
                name: "RequiredPositionID",
                table: "EmpPositionInProjects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "ca4e31d6-99dc-4008-86e5-d43c9c9de802", new DateTime(2021, 4, 12, 16, 26, 42, 873, DateTimeKind.Local).AddTicks(7704), "AQAAAAEAACcQAAAAEN+YGFPEk8tep28NLbYVNZ2PW/RmMHd1av6akkRikVT03uYeLtfvtm8sWfyl+Gn7pA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "4a4c3392-c512-49f2-ba05-9c3569cbd75f");
        }
    }
}
