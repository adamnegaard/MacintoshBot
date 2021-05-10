using Microsoft.EntityFrameworkCore.Migrations;

namespace MacintoshBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Channels",
                table => new
                {
                    RefName = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    ChannelId = table.Column<decimal>("numeric(20,0)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Channels", x => new {x.RefName, x.GuildId}); });

            migrationBuilder.CreateTable(
                "Groups",
                table => new
                {
                    Name = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    FullName = table.Column<string>("text", nullable: true),
                    IsGame = table.Column<bool>("boolean", nullable: false),
                    EmojiName = table.Column<string>("text", nullable: false),
                    DiscordRoleId = table.Column<decimal>("numeric(20,0)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Groups", x => new {x.Name, x.GuildId}); });

            migrationBuilder.CreateTable(
                "Images",
                table => new
                {
                    Title = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    Location = table.Column<string>("text", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Images", x => new {x.Title, x.GuildId}); });

            migrationBuilder.CreateTable(
                "LevelRoles",
                table => new
                {
                    RefName = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    RoleId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    Rank = table.Column<int>("integer", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_LevelRoles", x => new {x.RefName, x.GuildId}); });

            migrationBuilder.CreateTable(
                "Members",
                table => new
                {
                    UserId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    Xp = table.Column<int>("integer", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Members", x => new {x.UserId, x.GuildId}); });

            migrationBuilder.CreateTable(
                "Messages",
                table => new
                {
                    RefName = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    MessageId = table.Column<decimal>("numeric(20,0)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Messages", x => new {x.RefName, x.GuildId}); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Channels");

            migrationBuilder.DropTable(
                "Groups");

            migrationBuilder.DropTable(
                "Images");

            migrationBuilder.DropTable(
                "LevelRoles");

            migrationBuilder.DropTable(
                "Members");

            migrationBuilder.DropTable(
                "Messages");
        }
    }
}