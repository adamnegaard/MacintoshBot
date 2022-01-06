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
        private readonly RoleUpdateJob _roleUpdateJob;
        private readonly DisconnectChannelsJob _disconnectChannelsJob;
        private readonly IJobFactory _jobFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzHostedService> _logger;

        public QuartzHostedService(DailyFactJob dailyFactJob, RoleUpdateJob roleUpdateJob, DisconnectChannelsJob disconnectChannelsJob, IJobFactory jobFactory, ISchedulerFactory schedulerFactory, ILogger<QuartzHostedService> logger)
        {
            _dailyFactJob = dailyFactJob;
            _roleUpdateJob = roleUpdateJob;
            _disconnectChannelsJob = disconnectChannelsJob;
            _jobFactory = jobFactory;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }


        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            
            //Role updates
            var (roleUpdateJob, roleUpdateTrigger) = BuildJob(_roleUpdateJob, CronScheduleBuilder.DailyAtHourAndMinute(0, 0));
            await Scheduler.ScheduleJob(roleUpdateJob, roleUpdateTrigger, cancellationToken);
            
            //Daily facts
            var (dailyFactJob, dailyFactTrigger) = BuildJob(_dailyFactJob, CronScheduleBuilder.DailyAtHourAndMinute(12, 0));
            await Scheduler.ScheduleJob(dailyFactJob, dailyFactTrigger, cancellationToken);
            
            //Leave channels that have not been updated
            var (disconnectChannelsJob, disconnectChannelsTrigger) = BuildJob(_disconnectChannelsJob,  CronScheduleBuilder.CronSchedule("0 */1 * ? * *"));
            await Scheduler.ScheduleJob(disconnectChannelsJob, disconnectChannelsTrigger, cancellationToken);
            
            _logger.LogInformation("Scheduled the registered jobs");
            await Scheduler.Start(cancellationToken);
        }
        
        private static (IJobDetail jobDetail, ITrigger jobTrigger) BuildJob(IJob job, CronScheduleBuilder cronScheduleBuilder)
        {
            var jobDetail = JobBuilder
                .Create(job.GetType())
                .WithIdentity(job.GetType().FullName)
                .WithDescription(job.GetType().Name)
                .Build();

            var jobTrigger = TriggerBuilder
                .Create()
                .WithIdentity($"{job.GetType()}.trigger")
                .WithSchedule(cronScheduleBuilder)
                .Build();

            return (jobDetail, jobTrigger);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null) await Scheduler?.Shutdown(cancellationToken);
        }
    }
}