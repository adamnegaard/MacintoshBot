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

        public Task Create(ChannelDTO channel)
        {
            throw new System.NotImplementedException();
        }
    }
}