# Macintosh-Bot

## Setup

The bot uses [PostgreSQL](https://www.postgresql.org/).

Set your user secret for the database connection string:
```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=<ip>;Database=<dbname>;Username=<username>;Password=<pass>"
```
Find your bots Token under Bot><your_application>Build-A-Bot>Token [here](https://discord.com/developers/applications/), and set the token with:
```
dotnet user-secrets set "ConnectionStrings:DiscordClientSecret" "<token>"
```

## Building the database
Change to the root directory of the application `MacintoshBot` and enter the commands:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Necessary entries
The bot needs a few entries in the `Groups` and `Channels` tables to function properly:

#### Groups
To begin with, the bot will send a self assign message if none is already present. The role message will be filled based on the entires in the `Groups` table.
```
INSERT INTO public."Groups"(
	"Name", "GuildId", "FullName", "IsGame", "EmojiName", "DiscordRoleId")
	VALUES 
(<group-abbreviation>, <your_guild_id>, <group-full-name>, <boolean_is_game>, <emoji_name (for example ':computer:'>, <Role_id>),
...
...
(<group-abbreviation>, <your_guild_id>, <group-full-name>, <boolean_is_game>, <emoji_name (for example ':computer:'>, <Role_id>);
```

#### Channels
It needs an id of the roles channel (the channel of which the role message should be sent), an id of a new members channel to notify of leaves (usually same channel as where users are welcomed to the server) and an id of a daily facts channel to send daily facts.
```
INSERT INTO public."Channels"(
	"RefName", "GuildId", "ChannelId")
	VALUES
('role', <your_guild_id>, <role_channel_id>),
('newmembers', <your_guild_id>, <newmembers_channel_id>),
('dailyfacts', <your_guild_id>, <dailyfacts_channel_id>);
```
