using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Discord.Modules
{
    [Name("Invision Community")]
    // ReSharper disable once UnusedMember.Global
    public class IpsModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        private readonly HttpClient _httpClient;

        public IpsModule(IConfigurationRoot config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        [Command("findmember")]
        [Summary("Znajduje użytkownika forum po jego ID i wyświetla detale jego konta.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        // ReSharper disable once UnusedMember.Global
        public async Task FindMemberAsync(int id)
        {
            await ReplyAsync(id.ToString());
        }
        
        [Command("findmember")]
        [Summary("Znajduje użytkownika forum po jego nazwie i wyświetla detale jego konta.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        // ReSharper disable once UnusedMember.Global
        public async Task FindMemberAsync(string name)
        {
            var apiResponse = await _httpClient.GetStringAsync($"{_config["urls:apiBase"]}core/members?name=");
            await ReplyAsync(name);
        }
    }
}
