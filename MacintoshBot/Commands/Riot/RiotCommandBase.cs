using MacintoshBot.Models.User;
using RiotSharp.Interfaces;

namespace MacintoshBot.Commands.Riot
{
    public class RiotCommandBase : GameCommandBase
    {
        protected readonly IRiotApi _riotApi;
        protected readonly string version = "11.21.1";
        
        public RiotCommandBase(IUserRepository userRepository, IRiotApi riotApi) : base(
            userRepository)
        {
            _riotApi = riotApi;
        }

    }
}