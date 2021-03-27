using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class addProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeID",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectType", x => x.ID);
                });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "71abaf0e-7373-4e9d-823d-f5e4d80344c7", new DateTime(2021, 3, 26, 15, 31, 6, 427, DateTimeKind.Local).AddTicks(3821), "AQAAAAEAACcQAAAAEPTVqyeWSKlZ+GmgYOaPKWHDpwXHDyPAk7eFHsv/wZBupPqYpBzbN6BthHv1aNnU1g==" });

            migrationBuilder.InsertData(
                table: "ProjectType",
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
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "b40d5690-6252-4c43-9504-78dbf1e44a5c");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeID",
                table: "Projects",
                column: "ProjectTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectType_ProjectTypeID",
                table: "Projects",
                column: "ProjectTypeID",
                principalTable: "ProjectType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectType_ProjectTypeID",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectType");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectTypeID",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTypeID",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                columns: new[] { "ConcurrencyStamp", "DateCreated", "PasswordHash" },
                values: new object[] { "bf5bdbc7-cf14-4e35-acef-5dfd48d29455", new DateTime(2021, 3, 18, 12, 28, 19, 61, DateTimeKind.Local).AddTicks(527), "AQAAAAEAACcQAAAAED5aZaPMqgb717aFzSKLVeohTatoKuHi+CMAEdKdvvgEustFyJI2eT3Sr1CZleUnkw==" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                column: "ConcurrencyStamp",
                value: "db68debb-c9fc-4065-9751-f8bb0df3791c");
        }
    }
}
