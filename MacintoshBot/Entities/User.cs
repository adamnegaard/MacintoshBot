using System;
using System.ComponentModel.DataAnnotations;

namespace MacintoshBot.Entities
{
    public class User
    {
        [Key]
        public ulong UserId { get; set; }

        public int Xp { get; set; }

        public int Level => Xp / 100;
    }
}