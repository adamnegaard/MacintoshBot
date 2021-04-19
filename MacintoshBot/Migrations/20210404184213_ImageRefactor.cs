using Microsoft.EntityFrameworkCore.Migrations;

namespace MacintoshBot.Migrations
{
    public partial class ImageRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Images");

            migrationBuilder.CreateTable(
                "Files",
                table => new
                {
                    Title = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    Location = table.Column<string>("text", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Files", x => new {x.Title, x.GuildId}); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Files");

            migrationBuilder.CreateTable(
                "Images",
                table => new
                {
                    Title = table.Column<string>("text", nullable: false),
                    GuildId = table.Column<decimal>("numeric(20,0)", nullable: false),
                    Location = table.Column<string>("text", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Images", x => new {x.Title, x.GuildId}); });
        }
    }
}