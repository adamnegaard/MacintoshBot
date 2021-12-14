using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    
    [Table("roles")]
    public class Role
    {

        public Role()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("ref_name")]
        public string RefName { get; set; }
        
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Required]
        [Column("role_id")]
        public ulong RoleId { get; set; }

        [Required]
        [Column("rank")]
        public int Rank { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}