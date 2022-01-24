using System;
using System.Threading.Tasks;
using DSharpPlus;
using MacintoshBot.ClientHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MacintoshBot.Jobs
{
    //Followed tutorial at https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    public class DailyFactJob : IJob
    {
        private readonly IClientHandler _clientHandler;
        private readonly DiscordClient _client;
        private readonly ILogger<DailyFactJob> _logger;

        public DailyFactJob(IClientHandler clientHandler, DiscordClient client, ILogger<DailyFactJob> logger)
        {
            _clientHandler = clientHandler;
            _client = client;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
             await _clientHandler.DailyFact(_client);
             
            _logger.LogDebug($"Ran: {nameof(DailyFactJob)}");
        }
    }
}