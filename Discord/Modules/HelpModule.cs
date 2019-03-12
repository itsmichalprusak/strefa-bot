using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;

namespace Discord.Modules
{
    [Name("Pomoc")]
    // ReSharper disable once UnusedMember.Global
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public HelpModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("help")]
        [Summary("Pokazuje ogólną wiadomość pomocy.")]
        // ReSharper disable once UnusedMember.Global
        public async Task HelpAsync()
        {
            var prefix = _config["prefix"];
            var builder = new EmbedBuilder()
            {
                Color = new Color(255, 109, 0),
                Description = "Poniżej znajdziesz listę komend, których możesz użyć.\n" +
                              $"Wpisz `{prefix}help [komenda]`, by uzyskać pomoc nt. konkretnej komendy."
            };
            
            foreach (var module in _service.Modules)
            {
                if (module.Name is "Pomoc") continue;
                
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()} " +
                                       $"{string.Join(" ", cmd.Parameters.Select(p => "[" + p.Name + "]"))}" +
                                       $"\n";
                }
                
                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        [Summary("Pokazuje wiadomość pomocy dla konkretnej komendy.")]
        // ReSharper disable once UnusedMember.Global
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync("", false, Embeds.Error($"Komenda `{command}` nie istnieje."));
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(255, 109, 0),
                Description = $"Pomoc dla komendy `{command}`:"
            };

            var prefix = _config["prefix"];
            
            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Wariant: {prefix}{command} " +
                              $"{string.Join(" ", cmd.Parameters.Select(p => "[" + p.Name + "]"))}\n" + 
                              $"Opis: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
