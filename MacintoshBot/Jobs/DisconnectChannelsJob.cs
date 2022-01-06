using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
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
                // if no song is currently being played and the last update was more than five minutes ago, leave the voice channel
                if (conn.CurrentState.CurrentTrack == null && (now - lastUpdate).TotalMinutes >= 1)
                {
                    await conn.DisconnectAsync();
                    _logger.LogInformation($"Disconnected lavalink connection for guild with id {connectedGuildId}, due to inactivity for more than five minutes");
                }
            }
            _logger.LogInformation($"Ran: {nameof(DisconnectChannelsJob)}");
        }
    }
}