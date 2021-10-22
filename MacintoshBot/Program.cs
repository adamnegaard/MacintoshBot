using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MacintoshBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    //Get the enviroment
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    //Add the optional appsettings.json
                    builder.AddJsonFile($"appsettings.{env}.json", true);

                    //Add the user secrets for development environments
                    if (hostContext.HostingEnvironment.IsDevelopment()) builder.AddUserSecrets<Program>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureLogging((ctx, logging) =>
                        {
                            var directory = Directory.GetCurrentDirectory();
                            logging.AddFile(Path.Combine(directory, "Logs/MacintoshBot-{Date}.log"));
                            logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                        })
                        .UseWebRoot("Public")
                        //For some odd reason, this line is required for dependency injection in this case? See https://github.com/aspnet/DependencyInjection/issues/578
                        .UseDefaultServiceProvider(options =>
                            options.ValidateScopes = false);
                });
        }
    }
}