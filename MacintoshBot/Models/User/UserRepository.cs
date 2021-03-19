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
        private IDiscordContext _context;

        public UserRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task Create(ulong userId)
        {
            var newUser = new Entities.User
            {
                UserId = userId,
                Xp = 0,
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ulong userId)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser == null)
            {
                return;
            }
            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDTO>> Get()
        {
            return await _context.Users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Xp = u.Xp
            }).ToListAsync();
        }

        public async Task<int> AddXp(ulong userId, int xpAmout)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return 0;
            }
            user.Xp += xpAmout;
            await _context.SaveChangesAsync();
            return user.Xp;
        }

        public async Task<UserDTO> Get(ulong userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }
            return new UserDTO
            {
                UserId = user.UserId,
                Xp = user.Xp
            };
        }
    }
}