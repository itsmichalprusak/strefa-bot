using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Discord.Audio;
using VideoLibrary;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using YoutubeExtractor;

namespace Discord.Modules
{
    [Name("Muzyka")]
    // ReSharper disable once UnusedMember.Global
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        private readonly HttpClient _httpClient;

        private IAudioClient _audioClient;
        
        public MusicModule(IConfigurationRoot config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        [Command("testv")]
        [Summary("Test głosowy.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        // ReSharper disable once UnusedMember.Global
        public async Task FindMemberAsync(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync(
                "", false, Embeds.Error("Musisz dołączyć do kanału głosowego, albo #wzmienić " +
                                        "kanał, na który ma dołączyć bot.")); 
                return; }

            var audioClient = await channel.ConnectAsync();
            
            using (var ffmpeg = CreateStream("test.mp3"))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }
        
        [Command("yt")]
        [Summary("Test głosowy.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        // ReSharper disable once UnusedMember.Global
        public async Task YoutubeAsync(string url, IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync(
                "", false, Embeds.Error("Musisz dołączyć do kanału głosowego, albo #wzmienić " +
                                        "kanał, na który ma dołączyć bot.")); 
                return; }

            var client = new YoutubeClient();
            var video = await client.GetVideoAsync(url.Substring(url.Length - 11, 11));

            var title = video.Title; // "Infected Mushroom - Spitfire [Monstercat Release]"
            var author = video.Author; // "Monstercat"
            var duration = video.Duration; // 00:07:14
            
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(url.Substring(url.Length - 11, 11));
            
            var streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();
            var ext = streamInfo.Container.GetFileExtension();

            var path = $"{title}.{ext}";
            
            await client.DownloadMediaStreamAsync(streamInfo, path);

            var embed = new EmbedBuilder();
            embed.WithAuthor(author);
            embed.Title = title;
            embed.Description = $"{video.Statistics.LikeCount} / {video.Statistics.DislikeCount} / {video.Statistics.ViewCount}";
            embed.WithTimestamp(video.UploadDate);

            await ReplyAsync("", false, embed.Build());
            
            _audioClient = await channel.ConnectAsync();
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = _audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); _audioClient.Dispose();}
            }
        }

        [Command("stop")]
        [Summary("Zatrzymuje odtwarzanie muzyki.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task StopAsync()
        {
            _audioClient.Dispose();
        }
        
        private static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
    }
}
