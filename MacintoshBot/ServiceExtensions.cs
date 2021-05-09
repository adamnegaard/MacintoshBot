using System;
using System.IO;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MacintoshBot.Commands;
using MacintoshBot.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MacintoshBot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDiscordService(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var config = serviceProvider.GetService<ClientConfig>();
                if (config == null)
                {
                    throw new InvalidOperationException(
                        "Add a ClientConfig to the dependencies");
                    
                }
                
                var logger = serviceProvider.GetRequiredService<ILoggerFactory>();
                
                //Configuration for the client.
                var discordConfig = new DiscordConfiguration
                {
                    Token = config.Token,
                    TokenType = TokenType.Bot,
                    LoggerFactory = logger,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Debug,
                    //Setting the intents
                    Intents = DiscordIntents.GuildMembers
                              | DiscordIntents.AllUnprivileged
                              | DiscordIntents.GuildPresences
                };
                
                
                
                var client = new DiscordClient(discordConfig);

                //Return the service.
                return client;
            });


            return services.AddHostedService<Bot>();
        }
    }
}