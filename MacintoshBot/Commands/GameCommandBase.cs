using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;

namespace MacintoshBot.Commands
{
    public class GameCommandBase : BaseCommandModule
    {
        protected readonly IUserRepository _userRepository;

        public GameCommandBase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected async Task<(UserDTO user, DiscordMessage message, DiscordMember member)> GetStatsMessageAndUser(
            CommandContext ctx, DiscordMember member)
        {
            //Check if the member is null, if it is set the member to the one who queried.
            if (member == null) member = ctx.Member;
            var loadingMessage = await ctx.Channel.SendMessageAsync($"Getting {member.DisplayName}'s stats...");
            var user = await GetUserFromContext(ctx, member);
            if (user == null)
            {
                await loadingMessage.ModifyAsync($"Could not find user {member.DisplayName} in the database");
                return (null, loadingMessage, member);
            }

            return (user, loadingMessage, member); 
        }

        private async Task<UserDTO> GetUserFromContext(CommandContext ctx, DiscordMember member)
        {
            var guildId = ctx.Guild.Id;
            //Get the user from the database
            var (status, user) = await _userRepository.Get(member.Id, guildId);
            //If we cant find the user in the database, return null
            if (status != Status.Found)
            {
                return null;
            }

            return user; 
        }
    }
}