using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacintoshBot.Entities;

namespace MacintoshBot.Models.User
{
    public interface IUserRepository
    {
        Task Create(ulong userId, ulong guildId);
        Task Delete(ulong userId, ulong guildId);
        Task<IEnumerable<UserDTO>> Get(ulong guildId);
        Task<IEnumerable<UserDTO>> GetAll();
        Task<int> AddXp(ulong userId, ulong guildId, int xpAmout);
        Task<UserDTO> Get(ulong userId, ulong guildId);
    }
}