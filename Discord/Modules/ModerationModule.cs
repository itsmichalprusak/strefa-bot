using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;

namespace Discord.Modules
{
    [Name("Moderacja")]
    // ReSharper disable once UnusedMember.Global
    public class ModerationModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public ModerationModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("prune")]
        [Summary("Usuwa podaną ilość wiadomości z kanału.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        // ReSharper disable once UnusedMember.Global
        public async Task PruneAsync(int howMany)
        {
            var messages = await Context.Channel.GetMessagesAsync(howMany + 1).FlattenAsync();
            await ((ITextChannel) Context.Channel).DeleteMessagesAsync(messages);
            
            var confirmation = await ReplyAsync("", false, 
                Embeds.Ok($"Usunąłem `{howMany}` wiadomości z tego kanału."));
            
            await Task.Delay(2000);
            await confirmation.DeleteAsync();
        }
    }
}
