using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands
{
    [Description("Commands for linking various gaming profiles")]
    public class ConnectionCommands : BaseCommandModule
    {
        private readonly ISteamUser _steamUser;
        private readonly ISteamUserStats _steamUserStats;
        private readonly IUserRepository _userRepository;

        public ConnectionCommands(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface)
        {
            _userRepository = userRepository;
            _steamUser = steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
            _steamUserStats = steamInterface.CreateSteamWebInterface<SteamUserStats>(new HttpClient());
        }

        [Command(nameof(Link))]
        [Description("Link an account from a profile url")]
        public async Task Link(CommandContext ctx, Uri profilePage)
        {
            var guildId = ctx.Guild.Id;
            var memberId = ctx.Member.Id;

            var userUpdate = new UserUpdateDTO
            {
                GuildId = guildId,
                UserId = memberId
            };
            DiscordEmbedBuilder embed = null;
            if (profilePage.AbsoluteUri.ToLower().Contains("steam"))
                embed = await LinkSteam(userUpdate, profilePage.AbsoluteUri);

            if (embed == null)
                await ctx.Channel.SendMessageAsync($"Did not recognize the platform in the link: {profilePage}");

            await ctx.Channel.SendMessageAsync(MacintoshEmbed.Create(embed));
        }

        public async Task<DiscordEmbedBuilder> LinkSteam(UserUpdateDTO userUpdate, string profilePage)
        {
            try
            {
                var vainityPattern = @"steamcommunity.com\/id\/(.*)\/";
                var reg = new Regex(vainityPattern, RegexOptions.IgnoreCase);
                var vanityMatch = reg.Match(profilePage);
                if (vanityMatch.Success)
                {
                    var vanityName = vanityMatch.Groups[1].ToString();
                    var profile = await _steamUser.ResolveVanityUrlAsync(vanityName);
                    var steamId = profile.Data;
                    userUpdate.SteamId = steamId;
                    var (status, user) = await _userRepository.Update(userUpdate);
                    if (status == Status.Updated) return GetSteamSuccess(steamId);
                }

                return GetInvalidSteamFormat();
            }
            catch (HttpRequestException)
            {
                return GetSteamError();
            }
        }

        public static DiscordEmbedBuilder GetSteamSuccess(ulong steamId)
        {
            return new()
            {
                Title = "Success",
                Description = $"Successfully linked your Steam ID: {steamId} to your profile"
            };
        }

        public static DiscordEmbedBuilder GetSteamError()
        {
            return new()
            {
                Title = "Error",
                Description =
                    "Error when linking your Steam account. Please make sure it is public [here](https://steamcommunity.com/my/edit/settings)"
            };
        }

        private static DiscordEmbedBuilder GetInvalidSteamFormat()
        {
            return new()
            {
                Title = "Error",
                Description =
                    "Unknown error trying to link your Steam account. Please make sure the URL is formatted properly"
            };
        }
    }
}