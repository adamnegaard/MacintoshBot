using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Steam.Models.SteamPlayer;

namespace MacintoshBot.SteamStats
{
    public class RustStats : SteamStats
    {
        public RustStats(IEnumerable<UserStatModel> stats) : base(stats)
        { }

        [JsonProperty(PropertyName = "kill_player")]
        public double Kills { get; private set; }

        [JsonProperty(PropertyName = "deaths")]
        public double Deaths { get; private set; }

        [JsonProperty(PropertyName = "bullet_hit_player")]
        public double BulletsHitPlayer { get; private set; }

        [JsonProperty(PropertyName = "arrow_hit_player")]
        public double ArrowsHitPlayer { get; private set; }

        [JsonProperty(PropertyName = "headshot")]
        public double HeadShots { get; private set; }

        public double KD => Kills / Deaths;
    }
}