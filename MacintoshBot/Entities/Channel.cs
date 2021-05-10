using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Channel
    {
        public string RefName { get; set; }
        public ulong GuildId { get; set; }

        [Required] public ulong ChannelId { get; set; }
    }
}