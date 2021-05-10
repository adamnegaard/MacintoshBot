using DSharpPlus.CommandsNext;
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
    }
}