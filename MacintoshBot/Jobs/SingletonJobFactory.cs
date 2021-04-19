using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace MacintoshBot.Jobs
{
    //Followed tutorial at https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _services;

        public SingletonJobFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _services.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}