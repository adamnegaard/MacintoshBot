using System.Threading.Tasks;

namespace MacintoshBot.Models.Channel
{
    public interface IChannelRepository
    {
        Task<ulong> Get(string refName, ulong guildId);

        Task Create(ChannelDTO channel);
    }
}