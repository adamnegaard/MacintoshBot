using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacintoshBot.Entities;

namespace MacintoshBot.Models.User
{
    public interface IUserRepository
    {
        Task Create(ulong userId);
        Task Delete(ulong userId);
        Task<IEnumerable<UserDTO>> Get();
        Task<int> AddXp(ulong userId, int xpAmout);
        Task<UserDTO> Get(ulong userId);
    }
}