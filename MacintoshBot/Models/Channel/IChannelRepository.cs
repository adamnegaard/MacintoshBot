using System.Threading.Tasks;

namespace MacintoshBot.Models.Channel
{
    public interface IChannelRepository
    {
        Task<(Status status, ChannelDTO channel)> Get(string refName, ulong guildId);

        Task<(Status status, ChannelDTO channel)> Create(ChannelDTO channel);
    }
}