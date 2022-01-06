using Microsoft.Extensions.Configuration;

namespace MacintoshBot
{
    public class ClientConfig
    {
        public ClientConfig(IConfiguration configuration)
        {
            Token = configuration.GetConnectionString("DiscordClientSecret");
        }
        
        public string Prefix => "?";
        public string Token { get; private set; }
    }
}