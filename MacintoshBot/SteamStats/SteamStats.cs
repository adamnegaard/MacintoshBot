using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Steam.Models.SteamPlayer;

namespace MacintoshBot.SteamStats
{
    public class SteamStats
    {
        public SteamStats(IEnumerable<UserStatModel> stats)
        {
            foreach (var stat in stats)
            {
                var property = GetType().GetProperties().FirstOrDefault(p =>
                {
                    var attribute = p.GetCustomAttribute<JsonPropertyAttribute>();
                    if (attribute != null && attribute.PropertyName != null)
                    {
                        return attribute.PropertyName.Equals(stat.Name); 
                    }

                    return false;
                });
                if (property != null && property.CanWrite) property.SetValue(this, stat.Value, null);
            }
        }
    }
}