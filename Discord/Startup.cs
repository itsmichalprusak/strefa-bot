using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discord
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
        private static readonly HttpClient HttpClient = new HttpClient();

        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddYamlFile("_config.yml");
            Configuration = builder.Build();
        }

        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }

        private async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetRequiredService<LoggingService>();
            serviceProvider.GetRequiredService<CommandHandler>();

            await serviceProvider.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = RunMode.Async,
                }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<StartupService>()
                .AddSingleton<LoggingService>()
                .AddSingleton(Configuration)
                .AddSingleton(HttpClient);
        }
    }
}
