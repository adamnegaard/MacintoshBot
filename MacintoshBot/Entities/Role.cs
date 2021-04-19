using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Role
    {
        public string RefName { get; set; }
        public ulong GuildId { get; set; }

        [Required] public ulong RoleId { get; set; }

        [Required] public int Rank { get; set; }
    }
}