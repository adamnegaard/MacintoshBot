using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    [Table("channels")]
    public class Channel
    {
        public Channel()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("ref_name")]
        public string RefName { get; set; }
        
        [Column("guild_id")]
        public ulong GuildId { get; set; }
        
        [Required]
        [Column("channel_id")]
        public ulong ChannelId { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}