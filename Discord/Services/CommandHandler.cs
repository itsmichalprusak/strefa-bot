using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Discord.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }
        
        private async Task OnMessageReceivedAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Author.Id == _discord.CurrentUser.Id) return;

            var context = new SocketCommandContext(_discord, message);

            var argPosition = 0;
            if (message.HasStringPrefix(_config["prefix"], ref argPosition)
                || message.HasMentionPrefix(_discord.CurrentUser, ref argPosition))
            {
                await _commands.ExecuteAsync(context, argPosition, _provider);
            }
        }
    }
}
