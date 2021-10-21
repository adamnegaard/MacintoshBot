using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using MacintoshBot.SteamStats;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamPlayer;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands
{
    public class SteamCommandBase : GameCommandBase
    {
        protected readonly IPlayerService _steamPlayerService;
        protected readonly ISteamUser _steamUser;
        protected readonly ISteamUserStats _steamUserStats;

        public SteamCommandBase(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface) : base(
            userRepository)
        {
            _steamUser = steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
            _steamUserStats = steamInterface.CreateSteamWebInterface<SteamUserStats>(new HttpClient());
            _steamPlayerService = steamInterface.CreateSteamWebInterface<PlayerService>(new HttpClient());
        }

        protected async Task<(DiscordMessage message, DiscordEmbedBuilder embedBuilder, IEnumerable<UserStatModel> stats, OwnedGameModel game)> getGameEmbed(CommandContext ctx, uint gameId, string gameName,
            DiscordMember member = null)
        {
            var guildId = ctx.Guild.Id;
            //Check if the member is null, if it is set the member to the one who queried.
            if (member == null) member = ctx.Member;
            var loadingMessage = await ctx.Channel.SendMessageAsync($"Getting {member.DisplayName}'s stats...");
            //Get the user from the database
            var (status, user) = await _userRepository.Get(member.Id, guildId);
            //If we can find the user in the database, return
            if (status != Status.Found)
            {
                await loadingMessage.ModifyAsync($"Could not find user {member.DisplayName} in the database");
                return (loadingMessage, null, null, null);
            }

            var steamId = user.SteamId;
            if (steamId == 0u)
            {
                await loadingMessage.ModifyAsync($"{member.DisplayName} does not have a SteamId set");
                return (loadingMessage, null, null, null);
            }

            try
            {
                var steamOwnedGames = await _steamPlayerService.GetOwnedGamesAsync(steamId);
                var game = steamOwnedGames.Data.OwnedGames.FirstOrDefault(g => g.AppId == gameId);

                if (game == null)
                {
                    await loadingMessage.ModifyAsync($"{member.DisplayName} does not own {gameName}");
                    return (loadingMessage, null, null, null);
                }

                var steamProfile = await _steamUser.GetPlayerSummaryAsync(steamId);

                var gameStats =
                    await _steamUserStats.GetUserStatsForGameAsync(steamId, gameId);
                
                return (loadingMessage, new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = steamProfile.Data.Nickname,
                        IconUrl = steamProfile.Data.AvatarUrl,
                        Url = steamProfile.Data.ProfileUrl
                    },
                    Title = $"{steamProfile.Data.Nickname}'s {gameName} stats",
                    Url = steamProfile.Data.ProfileUrl,
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = steamProfile.Data.AvatarFullUrl
                    }
                }, gameStats.Data.Stats, game);
            } catch (HttpRequestException)
            {
                await loadingMessage.ModifyAsync(GetPrivateSteamProfileEmbed());
                return (loadingMessage, null, null, null);
            }
        }

        protected DiscordMessageBuilder GetPrivateSteamProfileEmbed()
        {
            return MacintoshEmbed.Create(new DiscordEmbedBuilder
            {
                Title = "Error",
                Description =
                    "The game details of the user might be private, it can be changed [here](https://steamcommunity.com/my/edit/settings)."
            });
        }
    }
}