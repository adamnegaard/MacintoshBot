using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Role
{
    public interface ILevelRoleRepository
    {
        Task Create(RoleDTO role);
        Task<RoleDTO> Get(string refName, ulong guildId);
        Task<RoleDTO> Get(ulong roleId, ulong guildId);
        Task<RoleDTO> GetHighestRank(ulong guildId);
        Task<RoleDTO> GetLowestRank(ulong guildId);
        Task<RoleDTO> GetLevelRoleFromTime(DateTimeOffset joinedSince, ulong guildId);
        Task<RoleDTO> GetLevelFromDiscordMember(DiscordMember member, ulong guildId);
        Task<RoleDTO> GetLevelNext(int rank, ulong guildId);
        Task<IEnumerable<RoleDTO>> GetAllLevelPrev(int rank, ulong guildId);
        Task<IEnumerable<RoleDTO>> GetAllLevelNext(int rank, ulong guildId);
        //This bottom method might not be relevant for the specific interface, but for the current design it is.
        int GetDays(DateTimeOffset joinedSince);
    }
}