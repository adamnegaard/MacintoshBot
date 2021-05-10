using System;
using System.Linq;
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
            var loadingMessage = await ctx.Channel.SendMessageAsync($"Getting {member.DisplayName}'s stats...");
            //Get the user from the database
            var (status, user) = await _userRepository.Get(member.Id, guildId);
            //If we can find the user in the database, return
            if (status != Status.Found)
            {
                await loadingMessage.ModifyAsync($"Could not find user {member.DisplayName} in the database");
                return;
            }

            var steamId = user.SteamId;
            if (steamId == 0u)
            {
                await loadingMessage.ModifyAsync($"{member.DisplayName} does not have a SteamId set");
                return;
            }
            try
            {
                var steamOwnedGames  = await _steamPlayerService.GetOwnedGamesAsync(steamId);
                var rustGame = steamOwnedGames.Data.OwnedGames.FirstOrDefault(g => g.AppId == rustGameId);

                if (rustGame == null)
                {
                    await loadingMessage.ModifyAsync($"{member.DisplayName} does not own Rust");
                    return;
                }

                var steamProfile = await _steamUser.GetPlayerSummaryAsync(steamId);

                var gameStats =
                    await _steamUserStats.GetUserStatsForGameAsync(steamId, rustGameId);

                var rustStats = new RustStats(gameStats.Data.Stats);
                var discordEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = steamProfile.Data.Nickname,
                        IconUrl = steamProfile.Data.AvatarUrl,
                        Url = steamProfile.Data.ProfileUrl
                    },
                    Title = $"{steamProfile.Data.Nickname}'s Rust stats",
                    Url = steamProfile.Data.ProfileUrl,
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = steamProfile.Data.AvatarFullUrl,
                        
                    },
                };
                
                discordEmbed.AddField("KD", $"{Math.Round(rustStats.KD, 2)}", true);
                discordEmbed.AddField("Kills", $"{rustStats.Kills}", true);
                discordEmbed.AddField("Deaths", $"{rustStats.Deaths}", true);
                
                discordEmbed.AddField("Headshots", $"{rustStats.HeadShots}", true);
                discordEmbed.AddField("Bullet hits on players", $"{rustStats.BulletsHitPlayer}",true);
                discordEmbed.AddField("Arrow hits on players", $"{rustStats.ArrowsHitPlayer}",true);
                
                discordEmbed.AddField("Total hours", $"{Math.Round(rustGame.PlaytimeForever.TotalHours)}");
                
                discordEmbed.AddField("Stones harvested", $"{rustStats.StonesHarvested}", true);
                discordEmbed.AddField("Wood harvested", $"{rustStats.WoodHarvested}",true);
                
                await loadingMessage.ModifyAsync(MacintoshEmbed.Create(discordEmbed));
            }
            catch (HttpRequestException)
            {
                await loadingMessage.ModifyAsync(GetPrivateSteamProfileEmbed());
            }
        }
    }
}