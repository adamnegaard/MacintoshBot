namespace MacintoshBot.Entities
{
    public class User
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public int Xp { get; set; }
        public int Level => Xp / 100;
        public ulong SteamId { get; set; }
    }
}