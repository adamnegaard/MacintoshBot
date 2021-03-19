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
        private IDiscordContext _context;

        public GroupRepository(IDiscordContext context)
        {
            _context = context;
        }

        public async Task<GroupDTO> Get(string name)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
            if (group == null)
            {
                return null;
            }

            return new GroupDTO
            {
                Name = group .Name,
                FullName = group .FullName,
                IsGame = group .IsGame,
                EmojiName = group .EmojiName,
                DiscordRoleId = group .DiscordRoleId,
                DiscordRole = group.DiscordRole
            };
        }

        public async Task<DiscordRole> GetFromEmoji(string emojiName)
        {
            var game = await _context.Groups.FirstOrDefaultAsync(g => g.EmojiName == emojiName);
            if (game == null)
            {
                return null;
            }

            return game.DiscordRole;
        }

        public async Task<IEnumerable<GroupDTO>> Get()
        {
            return await _context.Groups.Select(g => new GroupDTO
            {
                Name = g.Name,
                FullName = g.FullName,
                IsGame = g.IsGame,
                EmojiName = g.EmojiName,
                DiscordRoleId = g.DiscordRoleId,
                DiscordRole = g.DiscordRole
            }).ToListAsync();
        }

        public async Task<bool> Create(GroupDTO game)
        {
            var gameCreate = new Entities.Group
            {
                Name = game.Name,
                FullName = game.FullName,
                IsGame = game.IsGame,
                EmojiName = game.EmojiName,
                DiscordRoleId = game.DiscordRoleId,
                DiscordRole = game.DiscordRole
            };
            await _context.Groups.AddAsync(gameCreate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(string name)
        {
            var game = await _context.Groups.FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
            if (game == null)
            {
                return false;
            }
            _context.Groups.Remove(game);
            await _context.SaveChangesAsync();
            return true; 
        }
    }
}