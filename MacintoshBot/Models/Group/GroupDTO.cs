using System;
using DSharpPlus.Entities;

namespace MacintoshBot.Models.Group
{
    public class GroupDTO
    {
        
        public string Name { get; set; }
        public ulong GuildId { get; set; }
        
        private string _fullName;
        public string FullName
        {
            get => (_fullName == null || _fullName.Equals(String.Empty)) ? Name.Substring(0,1).ToUpper() + Name.Substring(1) : _fullName;
            set => _fullName = value;
        }
        public bool IsGame { get; set; }
        public string EmojiName { get; set; }
        public ulong DiscordRoleId { get; set; }
    }
}