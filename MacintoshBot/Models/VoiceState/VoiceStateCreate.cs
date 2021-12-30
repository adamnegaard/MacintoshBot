using System;

namespace MacintoshBot.Models.VoiceState
{
    public class VoiceStateCreate
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime EnteredTime { get; set; }
    }
}