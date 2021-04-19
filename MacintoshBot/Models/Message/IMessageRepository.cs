using System.Threading.Tasks;

namespace MacintoshBot.Models.Message
{
    public interface IMessageRepository
    {
        Task<(Status status, ulong messageId)> GetMessageId(string refName, ulong guildId);
        Task<(Status status, MessageDTO message)> Create(MessageDTO message);
    }
}