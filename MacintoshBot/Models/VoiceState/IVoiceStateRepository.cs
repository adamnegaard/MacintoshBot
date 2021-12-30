using System.Threading.Tasks;
using MacintoshBot.Models.Message;

namespace MacintoshBot.Models.VoiceState
{
    public interface IVoiceStateRepository
    {
        Task<(Status status, Entities.VoiceState message)> Create(VoiceStateCreate voiceStateCreate);
        Task<(Status status, Entities.VoiceState message)> Update(VoiceStateUpdate voiceStateUpdate);
    }
}