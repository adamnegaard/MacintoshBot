using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using MacintoshBot.SteamStats;
using SteamWebAPI2.Utilities;

namespace MacintoshBot.Commands
{
    [System.ComponentModel.Description("Commands related to Rust")]
    public class RustCommands : SteamCommandBase
    {
        private readonly uint rustGameId = 252490;
        private readonly string rustGameName = "Rust";

        public RustCommands(IUserRepository userRepository, ISteamWebInterfaceFactory steamInterface) : base(
            userRepository, steamInterface) { }

        [Command(nameof(RustStats))]
        [Description("Check your Rust stats")]
        public async Task RustStats(CommandContext ctx, DiscordMember member = null)
        {
            var (discordMessage, discordEmbed, statModel, game) = await getGameEmbed(ctx, rustGameId, rustGameName, member);

            if (discordEmbed == null || statModel == null || game == null)
            {
                return;
            }
            
            var rustStats = new RustStats(statModel); 

            discordEmbed.AddField("KD", $"{Math.Round(rustStats.KD, 2)}", true);
            discordEmbed.AddField("Kills", $"{rustStats.Kills}", true);
            discordEmbed.AddField("Deaths", $"{rustStats.Deaths}", true);

            discordEmbed.AddField("Headshots", $"{rustStats.HeadShots}", true);
            discordEmbed.AddField("Bullet hits on players", $"{rustStats.BulletsHitPlayer}", true);
            discordEmbed.AddField("Arrow hits on players", $"{rustStats.ArrowsHitPlayer}", true);

            discordEmbed.AddField("Total hours", $"{Math.Round(game.PlaytimeForever.TotalHours)}");

            await discordMessage.ModifyAsync(MacintoshEmbed.Create(discordEmbed));
        }
    }
}