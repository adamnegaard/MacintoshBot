using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using Microsoft.Extensions.Logging;
using NLog;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamPlayer;
using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands.Steam
{
    public class SteamCommandBase : GameCommandBase
    {
        private readonly IPlayerService _steamPlayerService;
        private readonly ISteamUser _steamUser;
        private readonly ISteamUserStats _steamUserStats;

        private readonly ILogger<SteamCommandBase> _logger;

        public SteamCommandBase(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface, ILogger<SteamCommandBase> logger) : base(
            userRepository)
        {
            _steamUser = steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
            _steamUserStats = steamInterface.CreateSteamWebInterface<SteamUserStats>(new HttpClient());
            _steamPlayerService = steamInterface.CreateSteamWebInterface<PlayerService>(new HttpClient());
            _logger = logger;
        }

        protected async
            Task<(DiscordMessage message, DiscordEmbedBuilder embedBuilder, IEnumerable<UserStatModel> stats,
                OwnedGameModel game)> GetSteamGameEmbed(CommandContext ctx, uint gameId, string gameName,
                DiscordMember member = null)
        {
            var (user, loadingMessage, taggedMember) = await GetStatsMessageAndUser(ctx, member);
            if(user == null)
            {
                return (loadingMessage, null, null, null);
            }

            var steamId = user.SteamId;
            if (!steamId.HasValue)
            {
                var errMessage = $"{taggedMember.DisplayName} does not have a SteamId set";
                
                _logger.LogError(errMessage);
                await loadingMessage.ModifyAsync(errMessage);
                return (loadingMessage, null, null, null);
            }

            try
            {
                var steamOwnedGames = await _steamPlayerService.GetOwnedGamesAsync(steamId.Value);
                var game = steamOwnedGames.Data.OwnedGames.FirstOrDefault(g => g.AppId == gameId);

                if (game == null)
                {
                    var errMessage = $"{taggedMember.DisplayName} does not own {gameName}";
                
                    _logger.LogError(errMessage);
                    await loadingMessage.ModifyAsync(errMessage);
                    return (loadingMessage, null, null, null);
                }

                var steamProfile = await _steamUser.GetPlayerSummaryAsync(steamId.Value);

                var gameStats =
                    await _steamUserStats.GetUserStatsForGameAsync(steamId.Value, gameId);
                
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

        protected DiscordEmbed GetPrivateSteamProfileEmbed()
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