using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;

namespace Discord.Modules
{
    [Name("Społeczność")]
    // ReSharper disable once UnusedMember.Global
    public class CommunityModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        public CommunityModule(IConfigurationRoot config)
        {
            _config = config;
        }

        [Command("announce")]
        [Summary("Tworzy nowe ogłoszenie na Discordzie.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        // ReSharper disable once UnusedMember.Global
        public async Task AnnounceAsync([Remainder] string text)
        {
            if (_config["channels:announcements"] == null)
            {
                await ReplyAsync("", false, Embeds.Error("Nie uzupełniono ID kanału ogłoszeń " +
                                                         "w pliku konfiguracyjnym. Nie mogę wysłać ogłoszenia."));
                return;
            }

            var embed = new EmbedBuilder()
            {
                Color = new Color(255, 109, 0),
                Title = "Ogłoszenie",
                Description = text
            }.WithCurrentTimestamp().Build();

            try
            {
                var channel = Context.Guild.GetTextChannel(ulong.Parse(_config["channels:announcements"]));

                if (channel == null)
                {
                    await ReplyAsync("", false, Embeds.Error("Nie mogę odnaleźć " +
                                                             "kanału ogłoszeń. Sprawdź, czy taki kanał istnieje."));
                    return;
                }
                
                await channel.SendMessageAsync("", false, embed);
            } 
            catch (Exception) 
            {
                await ReplyAsync("", false, Embeds.Error("Nie mogę odnaleźć kanału " +
                                                         "ogłoszeń. Sprawdź poprawność pliku konfiguracyjnego."));
            }
            
        }
    }
}
