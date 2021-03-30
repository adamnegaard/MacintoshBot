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
The bot needs a few entries in the `Messages` and `Channels` table to function properly:

#### Messages
To begin with, it needs an ID of a message that will be used in your guild for members to self assign their roles
```
INSERT INTO public."Messages"(
	"RefName", "GuildId", "MessageId")
VALUES ('role', <your_guild_id>, <role_message_id>);
```

#### Channels
It needs an id of the roles channel (the channel of which the role message is sent), an id of a new members channel to notify of leaves (usually same channel as where users are welcomed to the server) and an id of a daily facts channel to send daily facts.
```
INSERT INTO public."Channels"(
	"RefName", "GuildId", "ChannelId")
	VALUES
('role', <your_guild_id>, <role_channel_id>),
('newmembers', <your_guild_id>, <newmembers_channel_id>),
('dailyfacts', <your_guild_id>, <dailyfacts_channel_id>);
```