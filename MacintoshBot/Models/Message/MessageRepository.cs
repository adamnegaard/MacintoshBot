using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Message
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IDiscordContext _context;

        public MessageRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task<ulong> Get(string refName, ulong guildId)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.RefName.ToLower().Equals(refName.ToLower()) && m.GuildId == guildId);
            return message?.MessageId ?? 0;
        }

        public async Task Create(MessageDTO message)
        {
            var messageCreate = new Entities.Message
            {
                MessageId = message.MessageId,
                GuildId = message.GuildId,
                RefName = message.RefName,
            };
            await _context.Messages.AddAsync(messageCreate);
            await _context.SaveChangesAsync();
        }
    }
}