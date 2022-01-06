using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using MacintoshBot.ClientHandler;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MacintoshBot.Jobs
{
    public class DisconnectChannelsJob : IJob
    {
        private readonly DiscordClient _client;
        private readonly ILogger<DisconnectChannelsJob> _logger;

        public DisconnectChannelsJob(DiscordClient client, ILogger<DisconnectChannelsJob> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now; 
            var lava = _client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            var connectedGuilds = node.ConnectedGuilds;
            foreach (var connectedGuildId in connectedGuilds.Keys)
            {
                var conn = connectedGuilds.GetValueOrDefault(connectedGuildId);
                var lastUpdate = conn.CurrentState.LastUpdate;
                var minutesSinceLastUpdate = (now - lastUpdate).TotalMinutes; 
                // if there has not been an update for three or more minutes
                if (minutesSinceLastUpdate >= 3)
                {
                    // send a message to the channel
                    var message = await _client.SendMessageAsync(conn.Channel, $"Left {conn.Channel.Name} due to inactivity");
                    await message.CreateReactionAsync(DiscordEmoji.FromName(_client, ":wave:"));
                    
                    await conn.DisconnectAsync();
                    _logger.LogInformation($"Disconnected lavalink connection for guild with id {connectedGuildId}, due to inactivity for {minutesSinceLastUpdate} minutes");
                }
            }
            _logger.LogInformation($"Ran: {nameof(DisconnectChannelsJob)}");
        }
    }
}