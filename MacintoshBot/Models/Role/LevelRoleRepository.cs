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
                DiscordRoleId = role.DiscordRoleId,
                DiscordRole = role.DiscordRole,
                Rank = role.Rank,
            };
            await _context.LevelRoles.AddAsync(roleCreate);
            await _context.SaveChangesAsync();
        }

        public async Task<RoleDTO> Get(string refName)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r => r.RefName.ToLower().Equals(refName.ToLower()));
            if (role == null)
            {
                return null;
            }

            return new RoleDTO
            {
                RefName = role.RefName,
                DiscordRoleId = role.DiscordRoleId,
                DiscordRole = role.DiscordRole,
                Rank = role.Rank
            };
        }

        public async Task<RoleDTO> Get(ulong roleId)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r => r.DiscordRoleId == roleId);
            if (role == null)
            {
                return null;
            }

            return new RoleDTO
            {
                RefName = role.RefName,
                DiscordRoleId = role.DiscordRoleId,
                DiscordRole = role.DiscordRole,
                Rank = role.Rank
            };
        }

        public async Task<RoleDTO> GetHighestRank()
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r =>
                r.Rank == _context.LevelRoles.Max(r2 => r.Rank));
            
            if (role == null)
            {
                return null;
            }
            
            return new RoleDTO
            {
                RefName = role.RefName,
                DiscordRoleId = role.DiscordRoleId,
                DiscordRole = role.DiscordRole,
                Rank = role.Rank
            };
        }

        public async Task<RoleDTO> GetLowestRank()
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r =>
                r.Rank == _context.LevelRoles.Min(r2 => r.Rank));
            if (role == null)
            {
                return null;
            }
            return new RoleDTO
            {
                RefName = role.RefName,
                DiscordRoleId = role.DiscordRoleId,
                DiscordRole = role.DiscordRole,
                Rank = role.Rank
            };
        }
        
        public async Task<RoleDTO> GetLevelRoleFromTime(DateTimeOffset joinedSince)
        {
            var days = GetDays(joinedSince);
            if (days >= 365)
            {
                return (await GetAllLevel()).FirstOrDefault(l => l.Rank == 2);
            }
            else if (days >= 100)
            {
                return (await GetAllLevel()).FirstOrDefault(l => l.Rank == 1);
            }
            else
            {
                return (await GetAllLevel()).FirstOrDefault(l => l.Rank == 0);
            }
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevel()
        {
            return await _context.LevelRoles.Select(r => new RoleDTO
            {
                DiscordRoleId = r.DiscordRoleId,
                DiscordRole = r.DiscordRole,
                RefName = r.RefName,
                Rank = r.Rank
            }).ToListAsync();
        }

        public async Task<RoleDTO> GetLevelFromDiscordMember(DiscordMember member)
        {
            var role = (await GetAllLevel()).FirstOrDefault(r => member.Roles.Contains(r.DiscordRole));
            if (role == null)
            {
                return null;
            }
            return role; 
        }

        public async Task<RoleDTO> GetLevelNext(int rank)
        {
            var nextLevel = (await GetAllLevel()).FirstOrDefault(r => r.Rank == rank + 1);
            if (nextLevel == null)
            {
                return null;
            }

            return nextLevel;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevelPrev(int rank)
        {
            return (await GetAllLevel()).Where(r => r.Rank < rank).ToList();
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevelNext(int rank)
        {
            return (await GetAllLevel()).Where(r => r.Rank > rank).ToList();
        }
        
        public int GetDays(DateTimeOffset joinedSince)
        {
            return (DateTime.Now - joinedSince).Days;
        }
    }
}