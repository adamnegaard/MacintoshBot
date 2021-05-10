using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Mappings;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands
{
    public class SteamCommandBase : GameCommandBase
    {
        protected readonly ISteamUser _steamUser;
        protected readonly ISteamUserStats _steamUserStats;
        protected readonly IPlayerService _steamPlayerService;

        public SteamCommandBase(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface) : base(userRepository)
        {
            _steamUser = steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
            _steamUserStats = steamInterface.CreateSteamWebInterface<SteamUserStats>(new HttpClient());
            _steamPlayerService = steamInterface.CreateSteamWebInterface<PlayerService>(new HttpClient());
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