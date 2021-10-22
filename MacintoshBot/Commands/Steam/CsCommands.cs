using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models.User;
using MacintoshBot.SteamStats;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands.Steam
{
    [System.ComponentModel.Description("Commands related to Counter Strike")]
    public class CsCommands : SteamCommandBase
    {
        private const uint CsGameId = 730;
        private const string CsGameName = "CSGO";

        public CsCommands(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface) : base(
            userRepository, steamInterface) { }

        [Command(nameof(CsStats))]
        [Description("Check your Counter Strike stats")]
        public async Task CsStats(CommandContext ctx, DiscordMember member = null)
        {
            var (discordMessage, discordEmbed, statModel, game) = await GetSteamGameEmbed(ctx, CsGameId, CsGameName, member);

            if (discordEmbed == null || statModel == null || game == null)
            {
                return;
            }

            CsStats csStats = new CsStats(statModel);

            discordEmbed.AddField("Kills", $"{csStats.Kills}", true);
            discordEmbed.AddField("Deaths", $"{csStats.Deaths}", true);
            discordEmbed.AddField("Wins", $"{csStats.Wins}", true);
            
            discordEmbed.AddField("Damage done", $"{csStats.DamageDone}", true);
            discordEmbed.AddField("Accuracy", $"{Math.Round(csStats.Accuracy, 2)}", true);

            discordEmbed.AddField("Total hours", $"{Math.Round(game.PlaytimeForever.TotalHours)}");

            await discordMessage.ModifyAsync(MacintoshEmbed.Create(discordEmbed));
        }
    }
}