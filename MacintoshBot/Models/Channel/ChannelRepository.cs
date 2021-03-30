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
        
        public async Task<ulong> Get(string refName, ulong guildId)
        {
            var channel = await _context.Channels.FirstOrDefaultAsync(c => c.RefName.ToLower().Equals(refName.ToLower()) && c.GuildId == guildId);
            return channel?.ChannelId ?? 0;
        }

        public async Task<ChannelDTO> Create(ChannelDTO channel)
        {
            var channelCreate = new Entities.Channel
            {
                RefName = channel.RefName,
                GuildId = channel.GuildId,
                ChannelId = channel.ChannelId,
            };
            
            var createdChannel = await _context.Channels.AddAsync(channelCreate);
            await _context.SaveChangesAsync();
            if (createdChannel.Entity == null)
            {
                return null;
            }
            return new ChannelDTO
            {
                RefName = createdChannel.Entity.RefName,
                GuildId = createdChannel.Entity.ChannelId,
                ChannelId = createdChannel.Entity.ChannelId
            };
        }
    }
}