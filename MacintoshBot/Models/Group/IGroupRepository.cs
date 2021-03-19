using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Group
{
    public interface IGroupRepository
    {
        Task<GroupDTO> Get(string name);
        Task<DiscordRole> GetFromEmoji(string emojiName);
        Task<IEnumerable<GroupDTO>> Get();
        Task<bool> Create(GroupDTO game);
        Task<bool> Delete(string name); 
    }
}