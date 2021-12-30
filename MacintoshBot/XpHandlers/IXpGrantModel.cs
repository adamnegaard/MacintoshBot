using System;
using System.Threading.Tasks;

namespace MacintoshBot.XpHandlers
{
    public interface IXpGrantModel
    {
        Task EnterVoiceChannel(ulong memberId, ulong guildId);
        Task<int> ExitVoiceChannel(ulong memberId, ulong guildId);
        Task MoveVoiceChannel(ulong memberId, ulong guildId);
    }
}