﻿using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class Message
    {
        public ulong DiscordId { get; set; }
        public ulong GuildId { get; set; }
        [Required]
        public string RefName { get; set; }
    }
}