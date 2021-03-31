using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Facts
{
    public class FactRepository : IFactRepository
    {
        private readonly IDiscordContext _context;

        public FactRepository(IDiscordContext context)
        {
            _context = context;
        }

        public async Task<(Status status, FactDTO fact)> Create(string factText)
        {
            var factCreate = new Fact
            {
                Text = factText
            };

            var createdFact = await _context.Facts.AddAsync(factCreate);
            await _context.SaveChangesAsync();
            
            if (createdFact.Entity == null)
            {
                return (Status.Error, null);
            }
            return (Status.Created, new FactDTO
            {
                Id = createdFact.Entity.Id,
                Text = createdFact.Entity.Text,
            });
        }

        public async Task<(Status status, FactDTO fact)> Get(int factId)
        {
            if (factId == 0)
            {
                return await GetMostRecent();
            }
            var fact = await _context.Facts.FirstOrDefaultAsync(f => f.Id == factId);
            if (fact == null)
            {
                return (Status.BadRequest, null);
            }
            return (Status.Found, new FactDTO
            {
                Id = fact.Id,
                Text = fact.Text
            });
        }

        private async Task<(Status status, FactDTO fact)> GetMostRecent()
        {
            var latestFactId = await _context.Facts.MaxAsync(f => f.Id);
            return await Get(latestFactId);
        }
    }
}