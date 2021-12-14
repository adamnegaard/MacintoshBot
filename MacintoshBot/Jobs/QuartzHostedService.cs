using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace MacintoshBot.Jobs
{
    //Followed tutorial at https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    public class QuartzHostedService : IHostedService
    {
        private readonly DailyFactJob _dailyFactJob;
        private readonly IJobFactory _jobFactory;
        private readonly RoleUpdateJob _roleUpdateJob;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzHostedService> _logger;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory, IJobFactory jobFactory, RoleUpdateJob roleUpdateJob,
            DailyFactJob dailyFactJob, ILogger<QuartzHostedService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _roleUpdateJob = roleUpdateJob;
            _dailyFactJob = dailyFactJob;
            _logger = logger;
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            //Role updates
            var roleUpdateJob = JobBuilder
                .Create(_roleUpdateJob.GetType())
                .WithIdentity(_roleUpdateJob.GetType().FullName)
                .WithDescription(_roleUpdateJob.GetType().Name)
                .Build();

            var roleUpdateTrigger = TriggerBuilder
                .Create()
                .WithIdentity($"{_roleUpdateJob.GetType()}.trigger")
                .WithSchedule(
                    //This is where the timing for the schedule gets set, right now every day at 00:00 (midnight)
                    CronScheduleBuilder.DailyAtHourAndMinute(00, 00))
                .Build();
            await Scheduler.ScheduleJob(roleUpdateJob, roleUpdateTrigger, cancellationToken);

            //Daily facts
            var dailyFactJob = JobBuilder
                .Create(_dailyFactJob.GetType())
                .WithIdentity(_dailyFactJob.GetType().FullName)
                .WithDescription(_dailyFactJob.GetType().Name)
                .Build();

            var dailyFactTrigger = TriggerBuilder
                .Create()
                .WithIdentity($"{_dailyFactJob.GetType()}.trigger")
                .WithSchedule(
                    //This is where the timing for the schedule gets set, right now every day at 12:00 (noon)
                    CronScheduleBuilder.DailyAtHourAndMinute(12, 00))
                .Build();
            await Scheduler.ScheduleJob(dailyFactJob, dailyFactTrigger, cancellationToken);

            
            _logger.LogInformation("Scheduled the registered jobs");
            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null) await Scheduler?.Shutdown(cancellationToken);
        }
    }
}