using System;

namespace MacintoshBot.Models.VoiceState
{
    public class VoiceStateUpdate
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime? LeftTime { get; set; }
        public DateTime? MovedTime { get; set; }

        public Entities.VoiceState UpdateEntity(Entities.VoiceState voiceState)
        {
            if (LeftTime != null)
            {
                voiceState.LeftTime = LeftTime;
            }

            if (MovedTime != null)
            {
                voiceState.MovedTime = MovedTime;
            }

            return voiceState;
        }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(GuildId)}: {GuildId}, {nameof(LeftTime)}: {LeftTime}, {nameof(MovedTime)}: {MovedTime}";
        }
    }
}