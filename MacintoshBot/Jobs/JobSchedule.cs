using System;

namespace MacintoshBot.Jobs
{
    //Followed tutorial at https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
    }
}