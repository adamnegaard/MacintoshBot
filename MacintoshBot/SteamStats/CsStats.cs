using System.Collections.Generic;
using Newtonsoft.Json;
using Steam.Models.SteamPlayer;

namespace MacintoshBot.SteamStats
{
    public class CsStats : SteamStats
    {
        public CsStats(IEnumerable<UserStatModel> stats) : base(stats)
        { }
    
        [JsonProperty(PropertyName = "total_kills")]
        public double Kills { get; private set; }
        
        [JsonProperty(PropertyName = "total_deaths")]
        public double Deaths { get; private set; }
        
        [JsonProperty(PropertyName = "total_wins")]
        public double Wins { get; private set; }
        
        [JsonProperty(PropertyName = "total_damage_done")]
        public double DamageDone { get; private set; }
        
        [JsonProperty(PropertyName = "total_shots_fired")]
        private double ShotsFired { get; set; }
        
        [JsonProperty(PropertyName = "total_shots_hit")]
        private double ShotsHit { get; set; }
        
        public double Accuracy
        {
            get => (ShotsHit / ShotsFired) * 100;
        }
    }
}