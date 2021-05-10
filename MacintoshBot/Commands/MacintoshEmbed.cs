using System;
using DSharpPlus.Entities;

namespace MacintoshBot.Commands
{
    public class MacintoshEmbed
    {
        public static DiscordMessageBuilder Create(DiscordEmbedBuilder embed)
        {
            embed.Color = new DiscordColor("#7289da");
            embed.Timestamp = DateTimeOffset.Now;
            return new DiscordMessageBuilder
            {
                Embed = embed
            };
        }

        public static DiscordMessageBuilder ErrorEmbed(string errorMessage)
        {
            return Create(new DiscordEmbedBuilder
            {
                Title = "Error",
                Description = errorMessage
            });
        }
    }
}