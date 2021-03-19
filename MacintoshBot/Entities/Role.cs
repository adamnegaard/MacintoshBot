using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace MacintoshBot.Entities
{
    public class Role
    {
        [Key]
        public string RefName { get; set; }
        [Required]
        public ulong DiscordRoleId { get; set; }
        public DiscordRole DiscordRole { get; set; }
        public int Rank { get; set; }
        
    }
}