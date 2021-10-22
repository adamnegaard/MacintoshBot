using Microsoft.EntityFrameworkCore.Migrations;

namespace MacintoshBot.Migrations
{
    public partial class RiotSummonerNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SummonerName",
                table: "Members",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummonerName",
                table: "Members");
        }
    }
}
