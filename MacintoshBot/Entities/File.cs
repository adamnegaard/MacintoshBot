using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.CommandsNext.Attributes;

namespace MacintoshBot.Entities
{
    public class File
    {
        public string Title { get; set; }
        public ulong GuildId { get; set; }
        [Required]
        public string Location { get; set; }
    }
}