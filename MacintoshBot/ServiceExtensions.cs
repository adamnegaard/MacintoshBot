using System;
using DSharpPlus;
using MacintoshBot.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MacintoshBot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDiscordService(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var config = serviceProvider.GetService<ClientConfig>();
                if (config == null)
                    throw new InvalidOperationException(
                        "Add a ClientConfig to the dependencies");

                var logger = serviceProvider.GetRequiredService<ILoggerFactory>();

                //Configuration for the client.
                var discordConfig = new DiscordConfiguration
                {
                    Token = config.Token,
                    TokenType = TokenType.Bot,
                    LoggerFactory = logger,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Debug,
                    // might be a bad idea, but will ensure that it reconnects when internet is down
                    ReconnectIndefinitely = true,
                    //Setting the intents
                    Intents = DiscordIntents.GuildMembers
                              | DiscordIntents.AllUnprivileged
                              | DiscordIntents.GuildPresences
                };

                var client = new DiscordClient(discordConfig);

                //Return the service.
                return client;
            });

            return services.AddHostedService<Bot>();
        }
        
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                
                q.ScheduleJob<RoleUpdateJob>(trigger => trigger
                    .WithIdentity("trigger for updating user roles")
                    .WithDescription("job for sending updating user roles")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0))
                );
                
                q.ScheduleJob<CleanupJob>(trigger => trigger
                    .WithIdentity("trigger for cleaning up")
                    .WithDescription("job for cleaning up")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(1, 0))
                );
                
                q.ScheduleJob<DailyFactJob>(trigger => trigger
                    .WithIdentity("trigger for sending daily facts")
                    .WithDescription("job for sending daily facts")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(12, 0))
                );
                
                q.ScheduleJob<DisconnectChannelsJob>(trigger => trigger
                    .WithIdentity("trigger for disconnecting bots from voice channels when they are inactive")
                    .WithDescription("job for disconnecting bots from voice channels when they are inactive")
                    .WithSchedule(CronScheduleBuilder.CronSchedule("0 */5 * ? * *"))
                );
            });

            // Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
            return services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }
    }
}