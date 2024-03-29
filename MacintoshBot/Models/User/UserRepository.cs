﻿using System.Collections.Generic;
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

        public async Task<(Status status, UserDTO user)> Create(ulong userId, ulong guildId)
        {
            var existingUser = await Get(userId, guildId);
            if (existingUser.status == Status.Found) return (Status.Conflict, existingUser.user);
            var newUser = new Entities.User
            {
                UserId = userId,
                GuildId = guildId,
                Xp = 0
            };

            var createdUser = await _context.Members.AddAsync(newUser);
            await _context.SaveChangesAsync();

            if (createdUser.Entity == null) return (Status.Error, null);

            return (Status.Created, new UserDTO
            {
                UserId = createdUser.Entity.UserId,
                GuildId = createdUser.Entity.GuildId,
                Xp = createdUser.Entity.Xp
            });
        }

        public async Task<Status> Delete(ulong userId, ulong guildId)
        {
            var existingUser =
                await _context.Members.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
            if (existingUser == null) return Status.BadRequest;
            _context.Members.Remove(existingUser);
            await _context.SaveChangesAsync();
            return Status.Deleted;
        }

        public async Task<IEnumerable<UserDTO>> Get(ulong guildId)
        {
            return await _context.Members.Where(u => u.GuildId == guildId).Select(u => new UserDTO
            {
                UserId = u.UserId,
                GuildId = u.GuildId,
                Xp = u.Xp,
                SteamId = u.SteamId,
                SummonerName = u.SummonerName
            }).ToListAsync();
        }

        public async Task<IEnumerable<UserDTO>> Get()
        {
            return await _context.Members.Select(u => new UserDTO
            {
                UserId = u.UserId,
                GuildId = u.GuildId,
                Xp = u.Xp,
                SteamId = u.SteamId,
                SummonerName = u.SummonerName
            }).ToListAsync();
        }

        public async Task<(Status status, UserDTO user)> Update(UserUpdateDTO userUpdate)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u =>
                u.UserId == userUpdate.UserId && u.GuildId == userUpdate.GuildId);
            if (user == null) return (Status.BadRequest, null);

            if (userUpdate.SteamId != 0) user.SteamId = userUpdate.SteamId;
            if (userUpdate.SummonerName != null) user.SummonerName = userUpdate.SummonerName;

            await _context.SaveChangesAsync();
            return (Status.Updated, new UserDTO
            {
                UserId = user.UserId,
                GuildId = user.GuildId,
                Xp = user.Xp,
                SteamId = user.SteamId,
                SummonerName = user.SummonerName
            });
        }

        public async Task<int> AddXp(ulong userId, ulong guildId, int xpAmount)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
            if (user == null) return 0;
            user.Xp += xpAmount;
            await _context.SaveChangesAsync();
            return user.Xp;
        }

        public async Task<(Status status, UserDTO user)> Get(ulong userId, ulong guildId)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
            if (user == null) return (Status.BadRequest, null);
            return (Status.Found, new UserDTO
            {
                UserId = user.UserId,
                GuildId = user.GuildId,
                Xp = user.Xp,
                SteamId = user.SteamId,
                SummonerName = user.SummonerName
            });
        }
    }
}