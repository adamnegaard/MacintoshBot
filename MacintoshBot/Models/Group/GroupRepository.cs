using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<(Status status, GroupDTO group)> Get(string name, ulong guildId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g =>
                g.Name.ToLower().Equals(name.ToLower()) && g.GuildId == guildId);

            if (group == null) return (Status.BadRequest, null);

            return (Status.Found, new GroupDTO
            {
                Name = group.Name,
                GuildId = guildId,
                FullName = group.FullName,
                IsGame = group.IsGame,
                EmojiName = group.EmojiName,
                DiscordRoleId = group.DiscordRoleId
            });
        }

        public async Task<(Status status, ulong roleId)> GetRoleIdFromEmoji(string emojiName, ulong guildId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g =>
                g.EmojiName.Equals(emojiName) && g.GuildId == guildId);

            if (group == null) return (Status.BadRequest, 0);

            return (Status.Found, group.DiscordRoleId);
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

        public async Task<(Status status, GroupDTO group)> Create(GroupDTO game)
        {
            var existingGroup = await Get(game.Name, game.GuildId);
            if (existingGroup.status == Status.Found) return (Status.Conflict, existingGroup.@group);
            var groupCreate = new Entities.Group
            {
                Name = game.Name,
                GuildId = game.GuildId,
                FullName = game.FullName,
                IsGame = game.IsGame,
                EmojiName = game.EmojiName,
                DiscordRoleId = game.DiscordRoleId
            };

            var createdChannel = await _context.Groups.AddAsync(groupCreate);
            await _context.SaveChangesAsync();

            if (createdChannel.Entity == null) return (Status.Error, null);
            return (Status.Created, new GroupDTO
            {
                Name = createdChannel.Entity.Name,
                GuildId = createdChannel.Entity.GuildId,
                FullName = createdChannel.Entity.FullName,
                IsGame = createdChannel.Entity.IsGame,
                EmojiName = createdChannel.Entity.EmojiName,
                DiscordRoleId = createdChannel.Entity.DiscordRoleId
            });
        }

        public async Task<Status> Delete(string name, ulong guildId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g =>
                g.Name.ToLower().Equals(name.ToLower()) && g.GuildId == guildId);

            if (group == null) return Status.BadRequest;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return Status.Deleted;
        }
    }
}