using System.Collections.Generic;
using System.Threading.Tasks;

namespace MacintoshBot.Models.User
{
    public interface IUserRepository
    {
        Task<(Status status, UserDTO user)> Create(ulong userId, ulong guildId);
        Task<Status> Delete(ulong userId, ulong guildId);
        Task<(Status status, UserDTO user)> Get(ulong userId, ulong guildId);
        Task<IEnumerable<UserDTO>> Get(ulong guildId);
        Task<IEnumerable<UserDTO>> Get();
        Task<int> AddXp(ulong userId, ulong guildId, int xpAmount);
    }
}