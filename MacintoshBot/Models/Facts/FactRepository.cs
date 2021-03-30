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

        public async Task<FactDTO> Create(FactDTO fact)
        {
            var factCreate = new Fact
            {
                Text = fact.Text,
            };

            var createdFact = await _context.Facts.AddAsync(factCreate);
            await _context.SaveChangesAsync();
            
            if (createdFact.Entity == null)
            {
                return null;
            }
            return new FactDTO
            {
                Id = createdFact.Entity.Id,
                Text = createdFact.Entity.Text,
            };
        }

        public async Task<FactDTO> Get(int factId)
        {
            if (factId == 0)
            {
                return await GetMostRecent();
            }
            var fact = await _context.Facts.FirstOrDefaultAsync(f => f.Id == factId);
            if (fact == null)
            {
                return null;
            }
            return new FactDTO
            {
                Id = fact.Id,
                Text = fact.Text
            };
        }

        public async Task<FactDTO> GetMostRecent()
        {
            var latestFactId = await _context.Facts.MaxAsync(f => f.Id);
            return await Get(latestFactId);
        }
    }
}