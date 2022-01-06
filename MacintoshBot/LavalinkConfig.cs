using Microsoft.Extensions.Configuration;

namespace MacintoshBot
{
    public class LavalinkConfig
    {
        public LavalinkConfig(IConfiguration configuration)
        {
            Host = configuration["Lavalink:Host"];
            Port = configuration["Lavalink:Port"];
            Password = configuration["Lavalink:Password"];
        }
        
        public string Host { get; private set; }
        public string Port { get; private set; }
        public string Password { get; private set; }
    }
}