using System;
using System.Configuration;
using MacintoshBot.Entities;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Image;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using MacintoshBot.RoleUpdate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace MacintoshBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IDiscordContext, DiscordContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Database related
            services.AddScoped<ILevelRoleRepository, LevelRoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddSingleton<ClientConfig>();
            
            //Models
            services.AddScoped<IXpGrantModel, XpGrantModel>();
            services.AddScoped<IClientHandler, ClientHandler>();
            
            //The discord bot
            services.AddDiscordService();
              
            //Quarts scheduling services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
               
            // Add our job
            services.AddSingleton<RoleUpdateJob>();
            //The timing for our job gets set inside the class of QuartzHostedService
            services.AddSingleton<IJob, RoleUpdateJob>();
            
            services.AddHostedService<QuartzHostedService>();

            services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}