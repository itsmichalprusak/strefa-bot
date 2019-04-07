using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;

namespace Discord.Modules
{
    [Name("FiveM")]
    // ReSharper disable once UnusedMember.Global
    public class FivemModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        public FivemModule(IConfigurationRoot config)
        {
            _config = config;
        }

        [Command("restart")]
        [Summary("Planuje restart serwerów gry za podany czas.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        // ReSharper disable once UnusedMember.Global
        public async Task RestartAsync(int minutes)
        {
            if (minutes < 1 || minutes > 10)
            {
                await ReplyAsync("", false, Embeds.Error("Czas restartu musi mieścić " +
                                                         "się w przedziale od 1 do 10 minut."));
                return;
            }
            
            using (var process = new Process())
            {
                process.StartInfo.FileName = "screen";
                process.StartInfo.Arguments = $"-d /home/fivem/Common/restartServers.sh {minutes.ToString()}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                Console.WriteLine(process.StandardOutput.ReadToEnd());

                process.WaitForExit();
            }
        }
    }
}
