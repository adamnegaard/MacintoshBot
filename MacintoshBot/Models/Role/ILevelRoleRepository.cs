using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Role
{
    public interface ILevelRoleRepository
    {
        Task<(Status status, RoleDTO role)> Create(RoleDTO role);
        Task<(Status status, RoleDTO role)> Get(string refName, ulong guildId);
        Task<(Status status, RoleDTO role)> Get(ulong roleId, ulong guildId);
        Task<(Status status, RoleDTO role)> GetHighestRank(ulong guildId);
        Task<(Status status, RoleDTO role)> GetLowestRank(ulong guildId);
        Task<(Status status, RoleDTO role)> GetLevelRoleFromTime(DateTimeOffset joinedSince, ulong guildId);
        Task<(Status status, RoleDTO role)> GetLevelFromDiscordMember(DiscordMember member, ulong guildId);
        Task<(Status status, RoleDTO role)> GetLevelNext(int rank, ulong guildId);
        Task<IEnumerable<RoleDTO>> GetAllLevelPrev(int rank, ulong guildId);

        Task<IEnumerable<RoleDTO>> GetAllLevelNext(int rank, ulong guildId);

        //This bottom method might not be relevant for the specific interface, but for the current design it is.
        (Status status, int days) GetDays(DateTimeOffset joinedSince);
    }
}