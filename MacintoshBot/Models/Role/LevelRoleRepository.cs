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
        
        public async Task<(Status status, RoleDTO role)> Create(RoleDTO role)
        {
            var existingRole = await Get(role.RefName, role.GuildId);
            if (existingRole.status == Status.Found)
            {
                return (Status.Conflict, existingRole.role);
            }
            var roleCreate = new Entities.Role
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                RoleId = role.RoleId,
                Rank = role.Rank,
            };
            var createdRole = await _context.LevelRoles.AddAsync(roleCreate);
            await _context.SaveChangesAsync();

            if (createdRole.Entity == null)
            {
                return (Status.Error, role);
            }

            return (Status.Created, new RoleDTO
            {
                RefName = createdRole.Entity.RefName,
                GuildId = createdRole.Entity.GuildId,
                RoleId = createdRole.Entity.RoleId,
                Rank = createdRole.Entity.Rank,
            });
        }

        public async Task<(Status status, RoleDTO role)> Get(string refName, ulong guildId)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r => r.RefName.ToLower().Equals(refName.ToLower()) && r.GuildId == guildId);
            if (role == null)
            {
                return (Status.BadRequest, null);
            }

            return (Status.Found, new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                RoleId = role.RoleId,
                Rank = role.Rank
            });
        }

        public async Task<(Status status, RoleDTO role)> Get(ulong roleId, ulong guildId)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r => r.RoleId == roleId && r.GuildId == guildId);
            if (role == null)
            {
                return (Status.BadRequest, null);
            }

            return (Status.Found, new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                RoleId = role.RoleId,
                Rank = role.Rank
            });
        }

        public async Task<(Status status, RoleDTO role)> GetHighestRank(ulong guildId)
        {
            var roles = await GetAllLevel(guildId);
            if (!roles.Any())
            {
                return (Status.BadRequest, null);
            }
            var highestRank = roles.Max(r => r.Rank);

            var role = await _context.LevelRoles.FirstOrDefaultAsync(r =>
                r.Rank == highestRank);
            
            if (role == null)
            {
                return (Status.BadRequest, null);
            }
            
            return(Status.Found, new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                RoleId = role.RoleId,
                Rank = role.Rank
            });
        }

        public async Task<(Status status, RoleDTO role)> GetLowestRank(ulong guildId)
        {
            var roles = await GetAllLevel(guildId);
            if (!roles.Any())
            {
                return (Status.BadRequest, null);
            }
            var lowestRank = roles.Min(r => r.Rank);
            
            var role = await _context.LevelRoles.FirstOrDefaultAsync(r =>
                r.Rank == lowestRank);
            
            if (role == null)
            {
                return (Status.BadRequest, null);
            }
            
            return(Status.Found, new RoleDTO
            {
                RefName = role.RefName,
                GuildId = role.GuildId,
                RoleId = role.RoleId,
                Rank = role.Rank
            });
        }
        
        public async Task<(Status status, RoleDTO role)> GetLevelRoleFromTime(DateTimeOffset joinedSince, ulong guildId)
        {
            //these are the rules for leveling
            var result = GetDays(joinedSince);
            if (result.status != Status.Found)
            {
                return (Status.BadRequest, null);
            }
            var levelRoles = await GetAllLevel(guildId);
            if (!levelRoles.Any())
            {
                return (Status.BadRequest, null);
            }
            if (result.days >= 365)
            {
                var pro = levelRoles.FirstOrDefault(l => l.Rank == 2);
                if (pro != null)
                {
                    return (Status.Found, pro);
                }
            }
            if (result.days >= 100)
            {
                var intermediate = levelRoles.FirstOrDefault(l => l.Rank == 1);
                if (intermediate != null)
                {
                    return (Status.Found, intermediate);
                }
            }
            
            var scrub = levelRoles.FirstOrDefault(l => l.Rank == 0);
            if (scrub != null)
            {
                return (Status.Found, scrub);
            }
            
            return (Status.BadRequest, null);
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevel(ulong guildId)
        {
            return await _context.LevelRoles.Where(lr => lr.GuildId == guildId).Select(r => new RoleDTO
            {
                RoleId = r.RoleId,
                GuildId = r.GuildId,
                RefName = r.RefName,
                Rank = r.Rank
            }).ToListAsync();
        }

        public async Task<(Status status, RoleDTO role)> GetLevelFromDiscordMember(DiscordMember member, ulong guildId)
        {
            var role = (await GetAllLevel(guildId)).FirstOrDefault(r => member.Roles.Select(mr => mr.Id).Contains(r.RoleId));
            if (role == null)
            {
                return (Status.BadRequest, null);
            }
            
            return (Status.Found, role); 
        }

        public async Task<(Status status, RoleDTO role)> GetLevelNext(int rank, ulong guildId)
        {
            var nextLevel = (await GetAllLevel(guildId)).FirstOrDefault(r => r.Rank == rank + 1);
            
            if (nextLevel == null)
            {
                return (Status.BadRequest, null);
            }

            return (Status.Found, nextLevel);
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevelPrev(int rank, ulong guildId)
        {
            return (await GetAllLevel(guildId)).Where(r => r.Rank < rank).ToList();
        }

        public async Task<IEnumerable<RoleDTO>> GetAllLevelNext(int rank, ulong guildId)
        {
            return (await GetAllLevel(guildId)).Where(r => r.Rank > rank).ToList();
        }
        
        public (Status status, int days) GetDays(DateTimeOffset joinedSince)
        {
            var now = DateTime.Now;
            if (joinedSince > now)
            {
                return (Status.BadRequest, 0);
            }
            return (Status.Found, (now - joinedSince).Days);
        }
    }
}