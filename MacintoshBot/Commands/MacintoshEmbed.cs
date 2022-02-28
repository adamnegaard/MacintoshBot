using System;
using DSharpPlus.Entities;

namespace MacintoshBot.Commands
{
    public class MacintoshEmbed
    {
        public static DiscordEmbed Create(DiscordEmbedBuilder embed)
        {
            embed.Color = new DiscordColor("#7289da");
            embed.Timestamp = DateTimeOffset.Now;
            return embed.Build();
        }

        public static DiscordEmbed ErrorEmbed(string errorMessage)
        {
            return Create(new DiscordEmbedBuilder
            {
                Title = "Error",
                Description = errorMessage
            });
        }
    }
}