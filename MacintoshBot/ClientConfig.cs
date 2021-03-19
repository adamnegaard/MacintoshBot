using Microsoft.Extensions.Configuration;

namespace MacintoshBot
{
    public class ClientConfig
    {
        private string _token { get; }
        
        public ClientConfig(IConfiguration configuration)
        {
            _token = configuration.GetConnectionString("DiscordClientSecret");
        }
        public string Prefix => "?";
        public string Token => _token;
    }
}