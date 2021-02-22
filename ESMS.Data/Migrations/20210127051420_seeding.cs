using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESMS.Data.Migrations
{
    public partial class seeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SkillType",
                table: "Skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppConfigs",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfigs", x => x.Key);
                });

            migrationBuilder.InsertData(
                table: "AppConfigs",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "HomeTitle", "This is home page of ESMS system" },
                    { "HomeKeyword", "This is keyword of ESMS system" },
                    { "HomeDescription", "This is description of ESMS system" }
                });

            migrationBuilder.InsertData(
                table: "AppUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8D04DCE2-969A-435D-BBA4-DF3F325983DC", "69BD714F-9576-45BA-B5B7-F00649BE00DE" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "DateCreated", "DateEnd", "Email", "EmailConfirmed", "IdentityNumber", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "69BD714F-9576-45BA-B5B7-F00649BE00DE", 0, "580 Quang Trung P10", "6d0278b1-e6d1-4f14-86d5-93094f544493", new DateTime(2021, 1, 27, 12, 14, 19, 649, DateTimeKind.Local).AddTicks(7703), null, "resker123@gmail.com", true, "0123456789", false, null, "Pham Tuan", "resker123@gmail.com", "admin", "AQAAAAEAACcQAAAAEELGL/ir9ABuXHNWEZymwZZcVSLqzq+AC711aG5tdOGuAR7scjDAMxfo966uJlvmbw==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { "8D04DCE2-969A-435D-BBA4-DF3F325983DC", "9a5e54f7-0858-4434-9d55-86703b382450", "Administrator role", "admin", "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfigs");

            migrationBuilder.DeleteData(
                table: "AppUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8D04DCE2-969A-435D-BBA4-DF3F325983DC", "69BD714F-9576-45BA-B5B7-F00649BE00DE" });

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "69BD714F-9576-45BA-B5B7-F00649BE00DE");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8D04DCE2-969A-435D-BBA4-DF3F325983DC");

            migrationBuilder.DropColumn(
                name: "SkillType",
                table: "Skills");
        }
    }
}
