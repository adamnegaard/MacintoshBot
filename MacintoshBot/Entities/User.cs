using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacintoshBot.Entities
{
    [Table("users")]
    public class User
    {
        public User()
        {
            CreatedTime = DateTime.Now;
        }
        
        [Column("user_id")]
        public ulong UserId { get; set; }
        
        [Column("guild_id")]
        public ulong GuildId { get; set; }
        
        [Column("xp")]
        public int Xp { get; set; }
        
        public int Level => Xp / 100;
        
        [Column("steam_id")]
        public ulong? SteamId { get; set; }
        
        [Column("summoner_name")]
        public string? SummonerName { get; set; }
        
        [Column("created_ts")]
        public DateTime CreatedTime { get; set; }
    }
}