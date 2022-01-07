using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using MacintoshBot.ClientHandler;
using Microsoft.Extensions.Logging;
using NLog.Fluent;
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
                if ((conn.CurrentState.CurrentTrack == null && minutesSinceLastUpdate >= 5) || minutesSinceLastUpdate >= 10)
                {
                    await conn.DisconnectAsync();
                    _logger.LogInformation(
                        $"Disconnected lavalink connection for guild with id {connectedGuildId}, due to inactivity for {Convert.ToInt32(minutesSinceLastUpdate)} minutes");
                }
            }
            _logger.LogDebug($"Ran: {nameof(DisconnectChannelsJob)}");
        }
    }
}