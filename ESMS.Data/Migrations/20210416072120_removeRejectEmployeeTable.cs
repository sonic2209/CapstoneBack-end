using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class removeRejectEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_Positions_PosID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_Projects_ProjectID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_RequiredPositions_RequiredPositionID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropTable(
                name: "EmpPositions");

            migrationBuilder.DropTable(
                name: "RejectedEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmpPositionInProjects_EmpID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmpPositionInProjects_PosID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropColumn(
                name: "PosID",
                table: "EmpPositionInProjects");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredPositionID",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOut",
                table: "EmpPositionInProjects",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccept",
                table: "EmpPositionInProjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "EmpPositionInProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects",
                columns: new[] { "EmpID", "RequiredPositionID" });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "afcecd5e-9e6c-42dc-92b1-598e385c4a39", new DateTime(2021, 4, 16, 14, 21, 19, 713, DateTimeKind.Local).AddTicks(2471), "AQAAAAEAACcQAAAAEKB2lWmYFjH0BSCm2yUI12+AGkXVp9ZPglYXf7V8SDx/ZXu9efE4izZvv5NCoJXReA==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "c232273d-5533-4c0d-b137-dccc009d2767");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_RequiredPositions_RequiredPositionID",
                table: "EmpPositionInProjects",
                column: "RequiredPositionID",
                principalTable: "RequiredPositions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpPositionInProjects_RequiredPositions_RequiredPositionID",
                table: "EmpPositionInProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects");

            migrationBuilder.DropColumn(
                name: "IsAccept",
                table: "EmpPositionInProjects");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "EmpPositionInProjects");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOut",
                table: "EmpPositionInProjects",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequiredPositionID",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PosID",
                table: "EmpPositionInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpPositionInProjects",
                table: "EmpPositionInProjects",
                columns: new[] { "ProjectID", "PosID", "EmpID" });

            migrationBuilder.CreateTable(
                name: "EmpPositions",
                columns: table => new
                {
                    EmpID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PosID = table.Column<int>(type: "int", nullable: false),
                    DateIn = table.Column<DateTime>(type: "date", nullable: false),
                    DateOut = table.Column<DateTime>(type: "date", nullable: true),
                    PositionLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpPositions", x => new { x.EmpID, x.PosID });
                    table.ForeignKey(
                        name: "FK_EmpPositions_Employees_EmpID",
                        column: x => x.EmpID,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpPositions_Positions_PosID",
                        column: x => x.PosID,
                        principalTable: "Positions",
                        principalColumn: "PosID",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_EmpPositionInProjects_EmpID",
                table: "EmpPositionInProjects",
                column: "EmpID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositionInProjects_PosID",
                table: "EmpPositionInProjects",
                column: "PosID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpPositions_PosID",
                table: "EmpPositions",
                column: "PosID");

            migrationBuilder.CreateIndex(
                name: "IX_RejectedEmployees_RequiredPositionID",
                table: "RejectedEmployees",
                column: "RequiredPositionID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_Positions_PosID",
                table: "EmpPositionInProjects",
                column: "PosID",
                principalTable: "Positions",
                principalColumn: "PosID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_Projects_ProjectID",
                table: "EmpPositionInProjects",
                column: "ProjectID",
                principalTable: "Projects",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpPositionInProjects_RequiredPositions_RequiredPositionID",
                table: "EmpPositionInProjects",
                column: "RequiredPositionID",
                principalTable: "RequiredPositions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
