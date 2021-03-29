using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IDiscordContext _context;

        public UserRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task Create(ulong userId, ulong guildId)
        {
            var newUser = new Entities.User
            {
                UserId = userId,
                GuildId = guildId,
                Xp = 0,
            };
            await _context.Members.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ulong userId, ulong guildId)
        {
            var existingUser = await _context.Members.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
            if (existingUser == null)
            {
                return;
            }
            _context.Members.Remove(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDTO>> Get(ulong guildId)
        {
            return await _context.Members.Where(u => u.GuildId == guildId).Select(u => new UserDTO
            {
                UserId = u.UserId,
                GuildId = u.GuildId,
                Xp = u.Xp
            }).ToListAsync();
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            return await _context.Members.Select(u => new UserDTO
            {
                UserId = u.UserId,
                GuildId = u.GuildId,
                Xp = u.Xp
            }).ToListAsync();
        }

        public async Task<int> AddXp(ulong userId, ulong guildId, int xpAmout)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
            if (user == null)
            {
                return 0;
            }
            user.Xp += xpAmout;
            await _context.SaveChangesAsync();
            return user.Xp;
        }

        public async Task<UserDTO> Get(ulong userId, ulong guildId)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
            if (user == null)
            {
                return null;
            }
            return new UserDTO
            {
                UserId = user.UserId,
                GuildId = user.GuildId,
                Xp = user.Xp
            };
        }
    }
}