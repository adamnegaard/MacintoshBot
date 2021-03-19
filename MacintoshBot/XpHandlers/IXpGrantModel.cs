using System.Threading.Tasks;

namespace MacintoshBot
{
    public interface IXpGrantModel
    {
        void EnterVoiceChannel(ulong memberId);
        Task<int> ExitVoiceChannel(ulong memberId);
    }
}