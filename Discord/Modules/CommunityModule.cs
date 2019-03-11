using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;

namespace Discord.Modules
{
    [Name("Invision Community")]
    // ReSharper disable once UnusedMember.Global
    public class CommunityModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly IConfigurationRoot _config;

        public CommunityModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("findmember")]
        [Summary("Znajduje użytkownika forum po jego ID i wyświetla detale jego konta.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        // ReSharper disable once UnusedMember.Global
        public async Task PruneAsync(int memberId)
        {
            await ReplyAsync(memberId.ToString());
        }
    }
}
