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
        // ReSharper disable once UnusedMember.Global
        public async Task FindMemberAsync(int id)
        {
            await ReplyAsync(id.ToString());
        }
        
        [Command("findmember")]
        [Summary("Znajduje użytkownika forum po jego nazwie i wyświetla detale jego konta.")]
        [RequireContext(ContextType.Guild)]
        // ReSharper disable once UnusedMember.Global
        public async Task FindMemberAsync(string name)
        {
            await ReplyAsync(name);
        }
    }
}
