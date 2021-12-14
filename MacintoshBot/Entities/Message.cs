using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    [Table("messages")]
    public class Message
    {
        public Message()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("ref_name")]
        public string RefName { get; set; }
        
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Required]
        [Column("message_id")]
        public ulong MessageId { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}