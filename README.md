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
