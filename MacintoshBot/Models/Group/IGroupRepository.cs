using System.Collections.Generic;
using System.Threading.Tasks;

namespace MacintoshBot.Models.Group
{
    public interface IGroupRepository
    {
        Task<(Status status, GroupDTO group)> Get(string name, ulong guildId);
        Task<(Status status, ulong roleId)> GetRoleIdFromEmoji(string emojiName, ulong guildId);
        Task<IEnumerable<GroupDTO>> Get(ulong guildId);
        Task<(Status status, GroupDTO group)> Create(GroupDTO game);
        Task<Status> Delete(string name, ulong guildId);
    }
}