using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.CommandsNext.Attributes;

namespace MacintoshBot.Entities
{
    public class Image
    {
        [Key]
        public string Title { get; set; }
        
        [Required]
        public Uri Location { get; set; }
    }
}