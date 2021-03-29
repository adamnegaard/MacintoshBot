using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Role
{
    public class LevelRoleRepository : ILevelRoleRepository
    {
        private IDiscordContext _context;

        public LevelRoleRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task Create(RoleDTO role)
        {
            var roleCreate = new Entities.Role
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                DiscordRoleId = role.DiscordRoleId,
                Rank = role.Rank,
            };
            await _context.LevelRoles.AddAsync(roleCreate);
            await _context.SaveChangesAsync();
        }

        public async Task<RoleDTO> Get(string refName, ulong guildId)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r => r.RefName.ToLower().Equals(refName.ToLower()) && r.GuildId == guildId);
            if (role == null)
            {
                return null;
            }

            return new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                DiscordRoleId = role.DiscordRoleId,
                Rank = role.Rank
            };
        }

        public async Task<RoleDTO> Get(ulong roleId, ulong guildId)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r => r.DiscordRoleId == roleId && r.GuildId == guildId);
            if (role == null)
            {
                return null;
            }

            return new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                DiscordRoleId = role.DiscordRoleId,
                Rank = role.Rank
            };
        }

        public async Task<RoleDTO> GetHighestRank(ulong guildId)
        {
            var highestRank = _context.LevelRoles.Where(r => r.GuildId == guildId).Max(r => r.Rank);

            var role = await _context.LevelRoles.FirstOrDefaultAsync(r =>
                r.Rank == highestRank);
            
            if (role == null)
            {
                return null;
            }
            
            return new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                DiscordRoleId = role.DiscordRoleId,
                Rank = role.Rank
            };
        }

        public async Task<RoleDTO> GetLowestRank(ulong guildId)
        {
            var lowestRank = _context.LevelRoles.Where(r => r.GuildId == guildId).Min(r => r.Rank);
            
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r =>
                r.Rank == lowestRank);
            if (role == null)
            {
                return null;
            }
            return new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                DiscordRoleId = role.DiscordRoleId,
                Rank = role.Rank
            };
        }
        
        public async Task<RoleDTO> GetLevelRoleFromTime(DateTimeOffset joinedSince, ulong guildId)
        {
            var days = GetDays(joinedSince);
            if (days >= 365)
            {
                return (await GetAllLevel(guildId)).FirstOrDefault(l => l.Rank == 2);
            }
            if (days >= 100)
            {
                return (await GetAllLevel(guildId)).FirstOrDefault(l => l.Rank == 1);
            }
            
            return (await GetAllLevel(guildId)).FirstOrDefault(l => l.Rank == 0);
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevel(ulong guildId)
        {
            return await _context.LevelRoles.Where(lr => lr.GuildId == guildId).Select(r => new RoleDTO
            {
                DiscordRoleId = r.DiscordRoleId,
                GuildId = r.GuildId,
                RefName = r.RefName,
                Rank = r.Rank
            }).ToListAsync();
        }

        public async Task<RoleDTO> GetLevelFromDiscordMember(DiscordMember member, ulong guildId)
        {
            var role = (await GetAllLevel(guildId)).FirstOrDefault(r => member.Roles.Select(mr => mr.Id).Contains(r.DiscordRoleId));
            if (role == null)
            {
                return null;
            }
            return role; 
        }

        public async Task<RoleDTO> GetLevelNext(int rank, ulong guildId)
        {
            var nextLevel = (await GetAllLevel(guildId)).FirstOrDefault(r => r.Rank == rank + 1);
            if (nextLevel == null)
            {
                return null;
            }

            return nextLevel;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevelPrev(int rank, ulong guildId)
        {
            return (await GetAllLevel(guildId)).Where(r => r.Rank < rank).ToList();
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevelNext(int rank, ulong guildId)
        {
            return (await GetAllLevel(guildId)).Where(r => r.Rank > rank).ToList();
        }
        
        public int GetDays(DateTimeOffset joinedSince)
        {
            return (DateTime.Now - joinedSince).Days;
        }
    }
}