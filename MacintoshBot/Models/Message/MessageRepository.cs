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

        public async Task<(Status status, ulong messageId)> GetMessageId(string refName, ulong guildId)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m =>
                m.RefName.ToLower().Equals(refName.ToLower()) && m.GuildId == guildId);
            if (message == null) return (Status.BadRequest, 0);
            return (Status.Found, message.MessageId);
        }

        public async Task<(Status status, MessageDTO message)> Create(MessageDTO message)
        {
            var existingMessage = await GetMessageId(message.RefName, message.GuildId);
            if (existingMessage.status == Status.Found) return (Status.Conflict, null);
            var messageCreate = new Entities.Message
            {
                MessageId = message.MessageId,
                GuildId = message.GuildId,
                RefName = message.RefName
            };
            var createdMessage = await _context.Messages.AddAsync(messageCreate);
            await _context.SaveChangesAsync();

            if (createdMessage.Entity == null) return (Status.Error, null);

            return (Status.Created, new MessageDTO
            {
                MessageId = createdMessage.Entity.MessageId,
                GuildId = createdMessage.Entity.GuildId,
                RefName = createdMessage.Entity.RefName
            });
        }
    }
}