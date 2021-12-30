using System;
using System.ComponentModel.DataAnnotations.Schema;
using MacintoshBot.Models.VoiceState;

namespace MacintoshBot.Entities
{
    [Table("voice_states")]
    public class VoiceState
    {

        public VoiceState() { }
        public VoiceState(VoiceStateCreate voiceStateStateCreate)
        {
            UserId = voiceStateStateCreate.UserId;
            GuildId = voiceStateStateCreate.GuildId;
            EnteredTime = voiceStateStateCreate.EnteredTime;
        }

        [Column("id")]
        public int Id { get; set; }
        
        [Column("user_id")]
        public ulong UserId { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }
        
        [Column("entered_ts")]
        public DateTime EnteredTime { get; set; }
        
        [Column("left_ts")]
        public DateTime? LeftTime { get; set; }
        
        [Column("moved_time")]
        public DateTime? MovedTime { get; set; }
        
        public int GetTotalMinutes =>  Convert.ToInt32((LeftTime - EnteredTime)?.TotalMinutes);
    }
}