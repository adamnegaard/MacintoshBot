using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Group
    {
        public string Name { get; set; }
        public ulong GuildId { get; set; }
        public string FullName { get; set; }
        public bool IsGame { get; set; }

        [Required] public string EmojiName { get; set; }

        [Required] public ulong DiscordRoleId { get; set; }
    }
}