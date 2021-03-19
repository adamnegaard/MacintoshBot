using System;
using MacintoshBot.Entities;

namespace MacintoshBot.Models.User
{
    public class UserDTO
    {
        public ulong UserId { get; set; }
        public int Xp { get; set; }

        public int Level => Xp / 100;
    }
}