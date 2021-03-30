using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Channel
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly IDiscordContext _context;

        public ChannelRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task<ChannelDTO> Get(string refName, ulong guildId)
        {
            var channel = await _context.Channels.FirstOrDefaultAsync(c => c.RefName.ToLower().Equals(refName.ToLower()) && c.GuildId == guildId);
            if (channel == null)
            {
                return null;
            }
            return new ChannelDTO
            {
                RefName = channel.RefName,
                GuildId = channel.GuildId,
                ChannelId = channel.ChannelId
            };
        }

        public async Task<Status> Create(ChannelDTO channel)
        {
            var existingChannel = await Get(channel.RefName, channel.GuildId);
            if (existingChannel != null)
            {
                return Status.Conflict;
            }
            
            var channelCreate = new Entities.Channel
            {
                RefName = channel.RefName,
                GuildId = channel.GuildId,
                ChannelId = channel.ChannelId,
            };
            
            await _context.Channels.AddAsync(channelCreate);
            await _context.SaveChangesAsync();
            return Status.Created;
        }
    }
}