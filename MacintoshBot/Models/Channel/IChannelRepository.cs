using System.Threading.Tasks;

namespace MacintoshBot.Models.Channel
{
    public interface IChannelRepository
    {
        Task<ChannelDTO> Get(string refName, ulong guildId);

        Task<Status> Create(ChannelDTO channel);
    }
}