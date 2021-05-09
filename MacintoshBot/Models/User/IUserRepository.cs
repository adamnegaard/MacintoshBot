using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacintoshBot.Entities;

namespace MacintoshBot.Models.User
{
    public interface IUserRepository
    {
        Task<(Status status, UserDTO user)> Create(ulong userId, ulong guildId);
        Task<Status> Delete(ulong userId, ulong guildId);
        Task<(Status status, UserDTO user)> Get(ulong userId, ulong guildId);
        Task<IEnumerable<UserDTO>> Get(ulong guildId);
        Task<IEnumerable<UserDTO>> Get();
        Task<(Status status, UserDTO user)> Update(UserUpdateDTO userUpdate);
        Task<int> AddXp(ulong userId, ulong guildId, int xpAmount);
    }
}