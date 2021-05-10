using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Steam.Models.SteamPlayer;

namespace MacintoshBot.SteamStats
{
    public class RustStats
    {
        [JsonProperty(PropertyName = "kill_player")]
        public double Kills { get; private set; }
        [JsonProperty(PropertyName = "deaths")]
        public double Deaths { get; private set; }
        [JsonProperty(PropertyName = "bullet_hit_player")]
        public double BulletsHitPlayer { get; private set; }
        [JsonProperty(PropertyName = "arrow_hit_player")]
        public double ArrowsHitPlayer { get; private set; }
        [JsonProperty(PropertyName = "harvest.stones")]
        public double StonesHarvested { get; private set; }
        [JsonProperty(PropertyName = "harvest.wood")]
        public double WoodHarvested { get; private set; }
        [JsonProperty(PropertyName = "headshot")]
        public double HeadShots { get; private set; }
        [JsonProperty(PropertyName = "kd")]
        public double KD
        {
            get => Kills / Deaths;
        }

        public RustStats(IEnumerable<UserStatModel> stats)
        {
            foreach (var stat in stats)
            {
                var property = GetType().GetProperties().FirstOrDefault(p =>
                    p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName.Equals(stat.Name));
                if (property != null && property.CanWrite)
                {
                    property.SetValue(this, stat.Value, null);
                }
            }
        }
    }
}