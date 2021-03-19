using Microsoft.EntityFrameworkCore.Migrations;

namespace MacintoshBot.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_DiscordRole_DiscordRoleId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Groups_DiscordRoleId",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_DiscordRoleId",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "LevelRoles");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_RefName",
                table: "LevelRoles",
                newName: "IX_LevelRoles_RefName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LevelRoles",
                table: "LevelRoles",
                column: "RefName");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DiscordRoleId",
                table: "Groups",
                column: "DiscordRoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LevelRoles_DiscordRoleId",
                table: "LevelRoles",
                column: "DiscordRoleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelRoles_DiscordRole_DiscordRoleId",
                table: "LevelRoles",
                column: "DiscordRoleId",
                principalTable: "DiscordRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LevelRoles_DiscordRole_DiscordRoleId",
                table: "LevelRoles");

            migrationBuilder.DropIndex(
                name: "IX_Groups_DiscordRoleId",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LevelRoles",
                table: "LevelRoles");

            migrationBuilder.DropIndex(
                name: "IX_LevelRoles_DiscordRoleId",
                table: "LevelRoles");

            migrationBuilder.RenameTable(
                name: "LevelRoles",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_LevelRoles_RefName",
                table: "Roles",
                newName: "IX_Roles_RefName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "RefName");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DiscordRoleId",
                table: "Groups",
                column: "DiscordRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DiscordRoleId",
                table: "Roles",
                column: "DiscordRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_DiscordRole_DiscordRoleId",
                table: "Roles",
                column: "DiscordRoleId",
                principalTable: "DiscordRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
