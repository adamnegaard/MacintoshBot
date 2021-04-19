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

        public async Task<(Status status, ChannelDTO channel)> Get(string refName, ulong guildId)
        {
            var channel = await _context.Channels.FirstOrDefaultAsync(c =>
                c.RefName.ToLower().Equals(refName.ToLower()) && c.GuildId == guildId);
            if (channel == null) return (Status.BadRequest, null);
            return (Status.Found, new ChannelDTO
            {
                RefName = channel.RefName,
                GuildId = channel.GuildId,
                ChannelId = channel.ChannelId
            });
        }

        public async Task<(Status status, ChannelDTO channel)> Create(ChannelDTO channel)
        {
            var existingChannel = await Get(channel.RefName, channel.GuildId);
            if (existingChannel.status == Status.Found) return (Status.Conflict, existingChannel.channel);

            var channelCreate = new Entities.Channel
            {
                RefName = channel.RefName,
                GuildId = channel.GuildId,
                ChannelId = channel.ChannelId
            };

            var createdChannel = await _context.Channels.AddAsync(channelCreate);
            await _context.SaveChangesAsync();

            if (createdChannel.Entity == null) return (Status.Error, null);

            return (Status.Created, new ChannelDTO
            {
                ChannelId = createdChannel.Entity.ChannelId,
                GuildId = createdChannel.Entity.GuildId,
                RefName = createdChannel.Entity.RefName
            });
        }
    }
}