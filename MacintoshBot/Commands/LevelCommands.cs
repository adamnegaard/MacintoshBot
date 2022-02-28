using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using MacintoshBot.ClientHandler;
using MacintoshBot.Models;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;

namespace MacintoshBot.Commands
{
    public class LevelCommands : ApplicationCommandModule
    {
        private readonly IClientHandler _clientHandler;

        public LevelCommands(IClientHandler clientHandler)
        {
            _clientHandler = clientHandler;
        }
        
        [SlashCommand(nameof(Level), "See your current level and how many days you have been a member of the server")]
        public async Task Level(InteractionContext ctx, [Option("user", "User to see level of (empty if yourself)")] DiscordUser user = null)
        {
            var guildId = ctx.Guild.Id;
            
            var member = user == null ? ctx.Member : (DiscordMember) user;

            //Send the embed to the channel.
            var levelEmbed = await _clientHandler.GetLevelEmbed(ctx.Client, guildId, $"{member.DisplayName}'s profile", member);

            //await ctx.CreateResponseAsync(null);
            await ctx.CreateResponseAsync(levelEmbed);
        }
    }
}