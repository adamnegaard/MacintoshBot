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
    public class RoleUpdateJob : IJob
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RoleUpdateJob> _logger;

        public RoleUpdateJob(IServiceProvider services, ILogger<RoleUpdateJob> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _services.CreateScope())
            {
                var clientHandler = scope.ServiceProvider.GetService<IClientHandler>();
                var client = scope.ServiceProvider.GetService<DiscordClient>();
                if (clientHandler == null)
                {
                    return;
                }
                _logger.LogInformation("Got the services required for the job, attempting to update roles...");
                await clientHandler.EvaluateUserLevelUpdrades(client);
            }
        }
    }
}