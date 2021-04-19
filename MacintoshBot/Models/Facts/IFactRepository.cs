using System.Threading.Tasks;

namespace MacintoshBot.Models.Facts
{
    public interface IFactRepository
    {
        Task<(Status status, FactDTO fact)> Create(string factText);
        Task<(Status status, FactDTO fact)> Get(int factId);
    }
}