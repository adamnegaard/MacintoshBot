using DSharpPlus.Entities;

namespace MacintoshBot.Models.Role
{
    public class RoleDTO
    {
        public string RefName { get; set; }
        public ulong GuildId { get; set; }
        public ulong DiscordRoleId { get; set; }
        public int Rank { get; set; }
    }
}