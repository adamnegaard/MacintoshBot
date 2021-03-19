using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MacintoshBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordRole",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsHoisted = table.Column<bool>(type: "boolean", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Permissions = table.Column<long>(type: "bigint", nullable: false),
                    IsManaged = table.Column<bool>(type: "boolean", nullable: false),
                    IsMentionable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Title = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RefName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    JoinedSince = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Xp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    IsGame = table.Column<bool>(type: "boolean", nullable: false),
                    EmojiName = table.Column<string>(type: "text", nullable: false),
                    DiscordRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Groups_DiscordRole_DiscordRoleId",
                        column: x => x.DiscordRoleId,
                        principalTable: "DiscordRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RefName = table.Column<string>(type: "text", nullable: false),
                    DiscordRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RefName);
                    table.ForeignKey(
                        name: "FK_Roles_DiscordRole_DiscordRoleId",
                        column: x => x.DiscordRoleId,
                        principalTable: "DiscordRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Title", "Location" },
                values: new object[,]
                {
                    { "poggers", "http://themacs.dk/DiscordImages/poggers.jpg" },
                    { "big-spender", "http://themacs.dk/DiscordImages/big-spender.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "DiscordId", "RefName" },
                values: new object[] { 810921052939091988m, "role" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "JoinedSince", "Xp" },
                values: new object[,]
                {
                    { 234395307759108106m, new DateTime(2021, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 809373068676956220m, new DateTime(2021, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 255367450227507202m, new DateTime(2021, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 235025980073181184m, new DateTime(2021, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 267259924973748224m, new DateTime(2021, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 240570596105125889m, new DateTime(2021, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 296642397503488000m, new DateTime(2021, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 95 },
                    { 261086354828427264m, new DateTime(2021, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DiscordRoleId",
                table: "Groups",
                column: "DiscordRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_Title",
                table: "Images",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DiscordId",
                table: "Messages",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DiscordRoleId",
                table: "Roles",
                column: "DiscordRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RefName",
                table: "Roles",
                column: "RefName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DiscordRole");
        }
    }
}
