using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    [Table("files")]
    public class File
    {

        public File()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("title")]
        public string Title { get; set; }
        
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Required]
        [Column("location")]
        public string Location { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}