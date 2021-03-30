using System.Threading.Tasks;

namespace MacintoshBot.Models.Facts
{
    public interface IFactRepository
    {
        Task<FactDTO> Create(FactDTO fact);
        Task<FactDTO> Get(int factId);
        Task<FactDTO> GetMostRecent();
    }
}