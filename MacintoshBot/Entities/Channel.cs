using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Channels
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        [Required]
        public string RefName { get; set; }
    }
}