using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Services
{
    public class LoggingService
    {
        private string LogDirectory { get; }
        private string LogFile => Path.Combine(LogDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.log");

        // ReSharper disable once SuggestBaseTypeForParameter
        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {
            LogDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;
        }
        
        private Task OnLogAsync(LogMessage msg)
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            if (!File.Exists(LogFile))
                File.Create(LogFile).Dispose();

            var logText = $"{DateTime.UtcNow:hh:mm:ss} [{msg.Severity}] {msg.Source}: " +
                          $"{msg.Exception?.ToString() ?? msg.Message}";
            File.AppendAllText(LogFile, logText + "\n");

            return Console.Out.WriteLineAsync(logText);
        }
    }
}
