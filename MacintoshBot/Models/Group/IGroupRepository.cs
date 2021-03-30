using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Group
{
    public interface IGroupRepository
    {
        Task<GroupDTO> Get(string name, ulong guildId);
        Task<ulong> GetRoleIdFromEmoji(string emojiName, ulong guildId);
        Task<IEnumerable<GroupDTO>> Get(ulong guildId);
        Task<Status> Create(GroupDTO game);
        Task<Status> Delete(string name, ulong guildId); 
    }
}