using DSharpPlus;
using MacintoshBot.ClientHandler;
using MacintoshBot.Entities;
using MacintoshBot.Jobs;
using MacintoshBot.Models.Channel;
using MacintoshBot.Models.Facts;
using MacintoshBot.Models.File;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using MacintoshBot.XpHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using SteamWebAPI2.Utilities;

namespace MacintoshBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddDbContext<IDiscordContext, DiscordContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Database related
            services.AddScoped<ILevelRoleRepository, LevelRoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChannelRepository, ChannelRepository>();
            services.AddScoped<IFactRepository, FactRepository>();
            services.AddSingleton<ClientConfig>();

            //Models
            services.AddScoped<IXpGrantModel, XpGrantModel>();
            services.AddScoped<IClientHandler, ClientHandler.ClientHandler>();

            //Steam API related
            var webInterfaceFactory = new SteamWebInterfaceFactory(Configuration["Steam:ApiKey"]);
            services.AddSingleton<ISteamWebInterfaceFactory>(webInterfaceFactory);


            //The discord bot
            services.AddDiscordService();

            //Quarts scheduling services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            //The timing for our jobs gets set inside the class of QuartzHostedService
            services.AddSingleton<RoleUpdateJob>();
            services.AddSingleton<DailyFactJob>();

            services.AddHostedService<QuartzHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}