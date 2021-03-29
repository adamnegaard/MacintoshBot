using System.Threading.Tasks;

namespace MacintoshBot.Models.Message
{
    public interface IMessageRepository
    {
        Task<ulong> Get(string refName, ulong guildId);
        Task Create(MessageDTO message);
    }
}