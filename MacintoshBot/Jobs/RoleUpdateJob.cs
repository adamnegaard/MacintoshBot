using System;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MacintoshBot.Jobs
{
    //Followed tutorial at https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    public class RoleUpdateJob : IJob
    {
        private readonly IServiceProvider _services;

        public RoleUpdateJob(IServiceProvider services)
        {
            _services = services;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _services.CreateScope())
            {
                var clientHandler = scope.ServiceProvider.GetService<IClientHandler>();
                var client = scope.ServiceProvider.GetService<DiscordClient>();
                await clientHandler.EvaluateUserLevelUpdrades(client);
            }
        }
    }
}