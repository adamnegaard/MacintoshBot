using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Group
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDiscordContext _context;

        public GroupRepository(IDiscordContext context)
        {
            _context = context;
        }

        public async Task<GroupDTO> Get(string name, ulong guildId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name.ToLower().Equals(name.ToLower()) && g.GuildId == guildId);
            
            if (group == null)
            {
                return null;
            }

            return new GroupDTO
            {
                Name = group .Name,
                GuildId = guildId,
                FullName = group .FullName,
                IsGame = group .IsGame,
                EmojiName = group .EmojiName,
                DiscordRoleId = group .DiscordRoleId
            };
        }

        public async Task<ulong> GetFromEmoji(string emojiName, ulong guildId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.EmojiName.Equals(emojiName) && g.GuildId == guildId);
            
            if (group == null)
            {
                return 0;
            }

            return group.DiscordRoleId;
        }

        public async Task<IEnumerable<GroupDTO>> Get(ulong guildId)
        {
            return await _context.Groups.Where(g => g.GuildId == guildId).Select(g => new GroupDTO
            {
                Name = g.Name,
                GuildId = g.GuildId,
                FullName = g.FullName,
                IsGame = g.IsGame,
                EmojiName = g.EmojiName,
                DiscordRoleId = g.DiscordRoleId
            }).ToListAsync();
        }

        public async Task<bool> Create(GroupDTO game)
        {
            var existingGroup = await _context.Groups.FirstOrDefaultAsync(g =>
                g.Name.ToLower().Equals(game.Name.ToLower()) && g.GuildId == game.GuildId);
            if (existingGroup != null)
            {
                return false;
            }
            var groupCreate = new Entities.Group
            {
                Name = game.Name,
                GuildId = game.GuildId,
                FullName = game.FullName,
                IsGame = game.IsGame,
                EmojiName = game.EmojiName,
                DiscordRoleId = game.DiscordRoleId
            };
            await _context.Groups.AddAsync(groupCreate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(string name, ulong guildId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name.ToLower().Equals(name.ToLower()) && g.GuildId == guildId);
            
            if (group == null)
            {
                return false;
            }
            
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true; 
        }
    }
}