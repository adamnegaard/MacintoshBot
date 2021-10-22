namespace MacintoshBot.Models.User
{
    public class UserUpdateDTO
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public ulong SteamId { get; set; }
        public string SummonerName { get; set; }
    }
}