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

        public SteamCommandBase(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface) : base(userRepository)
        {
            _steamUser = steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
            _steamUserStats = steamInterface.CreateSteamWebInterface<SteamUserStats>(new HttpClient());
        }
        
        protected DiscordEmbedBuilder GetPrivateSteamProfileEmbed()
        {
            return new DiscordEmbedBuilder
            {
                Title = "Error",
                Description =
                    "The game details of the user might be private, it can be changed [here](https://steamcommunity.com/my/edit/settings)."
            };
        }

        public async Task<Status> Link(CommandContext ctx, string profilePage)
        {
            var guildId = ctx.Guild.Id;
            var memberId = ctx.Member.Id;
            
            var userUpdate = new UserUpdateDTO
            {
                GuildId = guildId,
                UserId = memberId,
            };
            
            if (profilePage.ToLower().Contains("steam"))
            {
                var profile = await _steamUser.ResolveVanityUrlAsync(profilePage, 1);
            }

            var (status, user) = await _userRepository.Update(userUpdate);
            return status;
        }
    }
}