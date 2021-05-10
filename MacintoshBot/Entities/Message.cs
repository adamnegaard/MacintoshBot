using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Message
    {
        public string RefName { get; set; }
        public ulong GuildId { get; set; }

        [Required] public ulong MessageId { get; set; }
    }
}