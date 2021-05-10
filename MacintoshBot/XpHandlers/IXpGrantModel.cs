using System;
using System.Threading.Tasks;

namespace MacintoshBot.XpHandlers
{
    public interface IXpGrantModel
    {
        void EnterVoiceChannel(ulong memberId, ulong guildId);
        Task<int> ExitVoiceChannel(ulong memberId, ulong guildId);

        Task<int> GetNewXpFromStartTime(DateTime startTime, ulong memberId, ulong guildId);
    }
}