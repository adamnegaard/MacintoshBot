using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Group
{
    public interface IGroupRepository
    {
        Task<GroupDTO> Get(string name, ulong guildId);
        Task<ulong> GetFromEmoji(string emojiName, ulong guildId);
        Task<IEnumerable<GroupDTO>> Get(ulong guildId);
        Task<bool> Create(GroupDTO game);
        Task<bool> Delete(string name, ulong guildId); 
    }
}