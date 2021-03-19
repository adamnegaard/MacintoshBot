using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace MacintoshBot.Entities
{
    public class Group
    {
        [Key]
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsGame { get; set; }
        [Required]
        public string EmojiName { get; set; }
        [Required]
        public ulong DiscordRoleId { get; set; }
        public DiscordRole DiscordRole { get; set; }
        
    }
}