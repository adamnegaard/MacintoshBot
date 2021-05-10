namespace MacintoshBot.Models.Group
{
    public class GroupDTO
    {
        private string _fullName;

        public string Name { get; set; }
        public ulong GuildId { get; set; }

        public string FullName
        {
            get => _fullName == null || _fullName.Equals(string.Empty)
                ? Name.Substring(0, 1).ToUpper() + Name.Substring(1)
                : _fullName;
            set => _fullName = value;
        }

        public bool IsGame { get; set; }
        public string EmojiName { get; set; }
        public ulong DiscordRoleId { get; set; }
    }
}