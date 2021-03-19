using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Message
    {
        [Key]
        public ulong DiscordId { get; set; }

        [Required]
        public string RefName { get; set; }
    }
}