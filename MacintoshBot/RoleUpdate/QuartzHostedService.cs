using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace MacintoshBot.RoleUpdate
{
    //Followed tutorial at https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IJob _job; 

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IJob job)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _job = job;
        }
        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            var job = JobBuilder
                .Create(_job.GetType())
                .WithIdentity(_job.GetType().FullName)
                .WithDescription(_job.GetType().Name)
                .Build();
            
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"{_job.GetType()}.trigger")
                .WithSchedule(
                    //This is where the timing for the schedule gets set, right now every day at 00:00 (midnight)
                    CronScheduleBuilder.DailyAtHourAndMinute(0, 0))
                .Build();

            await Scheduler.ScheduleJob(job, trigger, cancellationToken);

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null)
            {
                await Scheduler?.Shutdown(cancellationToken);
            }
        }
    }
}