using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Message
{
    public class MessageRepository : IMessageRepository
    {
        private IDiscordContext _context;

        public MessageRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task<ulong> Get(string refName)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.RefName.ToLower().Equals(refName.ToLower()));
            if (message == null)
            {
                return 0;
            }

            return message.DiscordId;
        }

        public async Task Create(MessageDTO message)
        {
            var messageCreate = new Entities.Message
            {
                DiscordId = message.DiscordId,
                RefName = message.RefName,
            };
            await _context.Messages.AddAsync(messageCreate);
            await _context.SaveChangesAsync();
        }
    }
}