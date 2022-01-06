using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.ClientHandler;
using MacintoshBot.Models;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;

namespace MacintoshBot.Commands
{
    [Description("Commands related to your server level")]
    public class LevelCommands : BaseCommandModule
    {
        private readonly IClientHandler _clientHandler;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IUserRepository _userRepository;

        public LevelCommands(IUserRepository userRepository, ILevelRoleRepository levelRoleRepository,
            IClientHandler clientHandler)
        {
            _userRepository = userRepository;
            _levelRoleRepository = levelRoleRepository;
            _clientHandler = clientHandler;
        }

        [Command(nameof(Level))]
        [Description("See your current level and how many days you have been a member of the server")]
        public async Task Level(CommandContext ctx, [Description("Member to see level of (empty if yourself)")]
            DiscordMember member = null)
        {
            var guildId = ctx.Guild.Id;
            //Check if the member is null, if it is set the member to the one who queried.
            if (member == null) member = ctx.Member;

            //Send the embed to the channel.
            var levelEmbed =
                await _clientHandler.GetLevelEmbed(ctx.Client, guildId, $"{member.DisplayName}'s profile", member);
            await ctx.RespondAsync(levelEmbed);
        }
    }
}