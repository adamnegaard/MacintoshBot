using System;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using MacintoshBot.SteamStats;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands
{
    [System.ComponentModel.Description("Commands related to Rust")]
    public class RustCommands : SteamCommandBase
    {
        private readonly uint rustGameId = 252490;
        
        public RustCommands(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface) : base(userRepository, steamInterface)
        {
        }
        
        [Command(nameof(RustStats))]
        [Description("Check your Rust stats")]
        public async Task RustStats(CommandContext ctx, DiscordMember member = null)
        {
            var guildId = ctx.Guild.Id;
            //Check if the member is null, if it is set the member to the one who queried.
            if (member == null)
            {
                member = ctx.Member;
            }
            //Get the user from the database
            var (status, user) = await _userRepository.Get(member.Id, guildId);
            //If we can find the user in the database, return
            if (status != Status.Found)
            {
                await ctx.Channel.SendMessageAsync($"Could not find user {member.DisplayName} in the database");
                return;
            }

            var steamId = user.SteamId;
            if (steamId == 0u)
            {
                await ctx.Channel.SendMessageAsync($"{member.DisplayName} does not have a SteamId set");
                return;
            }
            try
            {
                var steamProfile = await _steamUser.GetCommunityProfileAsync(steamId);
                
                var gameStats =
                    await _steamUserStats.GetUserStatsForGameAsync(steamId, rustGameId);

                var rustStats = new RustStats(gameStats.Data.Stats);
                var discordEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{steamProfile.CustomURL}'s Rust Stats",
                    Timestamp = DateTimeOffset.Now,
                    Color = DiscordColor.Aquamarine,
                    ImageUrl = steamProfile.AvatarFull.ToString()
                };
                
                discordEmbed.AddField("KDA", $"{rustStats.KDA}");
                discordEmbed.AddField("Kills", $"{rustStats.Kills}");
                discordEmbed.AddField("Headshots", $"{rustStats.HeadShots}");
                discordEmbed.AddField("Deaths", $"{rustStats.Deaths}");
                discordEmbed.AddField("Bullet hits on players", $"{rustStats.BulletsHitPlayer}");
                discordEmbed.AddField("Arrow hits on players", $"{rustStats.ArrowsHitPlayer}");
                discordEmbed.AddField("Stones harvested", $"{rustStats.StonesHarvested}");
                discordEmbed.AddField("Wood harvested", $"{rustStats.WoodHarvested}");
                
                await ctx.Channel.SendMessageAsync(embed: discordEmbed);
            }
            catch (HttpRequestException)
            {
                await ctx.Channel.SendMessageAsync(embed: GetPrivateSteamProfileEmbed());
            }
        }
    }
}