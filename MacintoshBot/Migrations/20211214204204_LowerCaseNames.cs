using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MacintoshBot.Migrations
{
    public partial class LowerCaseNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facts",
                table: "Facts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Channels",
                table: "Channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LevelRoles",
                table: "LevelRoles");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "messages");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "groups");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "files");

            migrationBuilder.RenameTable(
                name: "Facts",
                newName: "facts");

            migrationBuilder.RenameTable(
                name: "Channels",
                newName: "channels");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "LevelRoles",
                newName: "roles");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "messages",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "messages",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "RefName",
                table: "messages",
                newName: "ref_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "groups",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "IsGame",
                table: "groups",
                newName: "is_game");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "groups",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "EmojiName",
                table: "groups",
                newName: "emoji_name");

            migrationBuilder.RenameColumn(
                name: "DiscordRoleId",
                table: "groups",
                newName: "discord_role_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "groups",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "files",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "files",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "files",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "facts",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "facts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "channels",
                newName: "channel_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "channels",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "RefName",
                table: "channels",
                newName: "ref_name");

            migrationBuilder.RenameColumn(
                name: "Xp",
                table: "users",
                newName: "xp");

            migrationBuilder.RenameColumn(
                name: "SummonerName",
                table: "users",
                newName: "summoner_name");

            migrationBuilder.RenameColumn(
                name: "SteamId",
                table: "users",
                newName: "steam_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "users",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "roles",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "roles",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "RefName",
                table: "roles",
                newName: "ref_name");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "messages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "groups",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "files",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "facts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "channels",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_ts",
                table: "roles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_messages",
                table: "messages",
                columns: new[] { "ref_name", "guild_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_groups",
                table: "groups",
                columns: new[] { "name", "guild_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_files",
                table: "files",
                columns: new[] { "title", "guild_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_facts",
                table: "facts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_channels",
                table: "channels",
                columns: new[] { "ref_name", "guild_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                columns: new[] { "user_id", "guild_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                columns: new[] { "ref_name", "guild_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_groups",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_files",
                table: "files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_facts",
                table: "facts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_channels",
                table: "channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "files");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "facts");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_ts",
                table: "roles");

            migrationBuilder.RenameTable(
                name: "messages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "groups",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "files",
                newName: "Files");

            migrationBuilder.RenameTable(
                name: "facts",
                newName: "Facts");

            migrationBuilder.RenameTable(
                name: "channels",
                newName: "Channels");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Members");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "LevelRoles");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "Messages",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Messages",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "ref_name",
                table: "Messages",
                newName: "RefName");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Groups",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "is_game",
                table: "Groups",
                newName: "IsGame");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Groups",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "emoji_name",
                table: "Groups",
                newName: "EmojiName");

            migrationBuilder.RenameColumn(
                name: "discord_role_id",
                table: "Groups",
                newName: "DiscordRoleId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Groups",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "Files",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Files",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Files",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "Facts",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Facts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "channel_id",
                table: "Channels",
                newName: "ChannelId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Channels",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "ref_name",
                table: "Channels",
                newName: "RefName");

            migrationBuilder.RenameColumn(
                name: "xp",
                table: "Members",
                newName: "Xp");

            migrationBuilder.RenameColumn(
                name: "summoner_name",
                table: "Members",
                newName: "SummonerName");

            migrationBuilder.RenameColumn(
                name: "steam_id",
                table: "Members",
                newName: "SteamId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Members",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Members",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "rank",
                table: "LevelRoles",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "LevelRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "LevelRoles",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "ref_name",
                table: "LevelRoles",
                newName: "RefName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                columns: new[] { "RefName", "GuildId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                columns: new[] { "Name", "GuildId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                columns: new[] { "Title", "GuildId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facts",
                table: "Facts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Channels",
                table: "Channels",
                columns: new[] { "RefName", "GuildId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                columns: new[] { "UserId", "GuildId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LevelRoles",
                table: "LevelRoles",
                columns: new[] { "RefName", "GuildId" });
        }
    }
}
