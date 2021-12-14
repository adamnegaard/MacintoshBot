using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    [Table("groups")]
    public class Group
    {
        public Group()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("guild_id")]
        public ulong GuildId { get; set; }
        
        [Column("full_name")]
        public string FullName { get; set; }
        
        [Column("is_game")]
        public bool IsGame { get; set; }

        [Required]
        [Column("emoji_name")]
        public string EmojiName { get; set; }

        [Required] 
        [Column("discord_role_id")]
        public ulong DiscordRoleId { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}