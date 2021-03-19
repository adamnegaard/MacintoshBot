using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Role
{
    public interface ILevelRoleRepository
    {
        Task Create(RoleDTO role);
        Task<RoleDTO> Get(string refName);
        Task<RoleDTO> Get(ulong roleId);
        Task<RoleDTO> GetHighestRank();
        Task<RoleDTO> GetLowestRank();
        Task<RoleDTO> GetLevelRoleFromTime(DateTimeOffset joinedSince);
        Task<RoleDTO> GetLevelFromDiscordMember(DiscordMember member);
        Task<RoleDTO> GetLevelNext(int rank);
        Task<IEnumerable<RoleDTO>> GetAllLevelPrev(int rank);
        Task<IEnumerable<RoleDTO>> GetAllLevelNext(int rank);
        //This bottom method might not be relevant for the specific interface, but for the current design it is.
        int GetDays(DateTimeOffset joinedSince);
    }
}