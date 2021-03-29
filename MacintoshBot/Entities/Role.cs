using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace MacintoshBot.Entities
{
    public class Role
    {
        public string RefName { get; set; }
        public ulong GuildId { get; set; }
        [Required]
        public ulong DiscordRoleId { get; set; }
        [Required]
        public int Rank { get; set; }
        
    }
}