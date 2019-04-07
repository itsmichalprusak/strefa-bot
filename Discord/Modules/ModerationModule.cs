using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;
using Discord.Services;
using Discord.Utilities;
using Discord.WebSocket;
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

            if (!string.IsNullOrWhiteSpace(_config["log:prune:enable"]) && bool.Parse(_config["log:prune:enable"]))
            {
                var channel = Context.Guild.GetTextChannel(ulong.Parse(_config["log:prune:channel"]));
                
                if (channel == null)
                {
                    await ReplyAsync("", false, Embeds.Error(
                        "Nie odnalazłem kanału do wysłania logów moderacyjnych (a są one włączone)."));
                }
                else
                {
                    await channel.SendMessageAsync("", false, Embeds.ModLog(
                        ModerativeAction.Prune, 
                        $"{Context.Message.Author} / {Context.Message.Author.Mention}", 
                        $"#{Context.Channel.Name}")
                    );
                }
            }
            
            var confirmation = await ReplyAsync("", false, 
                Embeds.Ok($"Usunąłem `{howMany}` wiadomości z tego kanału."));
            
            await Task.Delay(2750);
            await confirmation.DeleteAsync();
        }

        [Command("kick")]
        [Summary("Wyrzuca użytkownika z serwera.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        // ReSharper disable once UnusedMember.Global
        public async Task KickAsync(SocketGuildUser user, [Remainder] string reason = "Nie podano.")
        {
            if (user == Context.User)
            {
                // We can't kick the invoker, because that wouldn't make much sense.
                await ReplyAsync("", false, Embeds.Error(
                    "Nie możesz wyrzucić samego siebie."));
                return;
            }
            
            if (user.Hierarchy >= ((SocketGuildUser) Context.User).Hierarchy)
            {
                // We can't kick someone who's more privileged than command invoker.
                await ReplyAsync("", false, Embeds.Error(
                    "Nie możesz wyrzucić osoby o wyższym ani równym poziomie uprawnień."));
                return;
            }

            if (user.Id == Context.Client.CurrentUser.Id)
            {
                // We can't kick the bot account we're operating on - again, what's the sense in that?
                await ReplyAsync("", false, Embeds.Error(
                    "Umm... Nie mogę wyrzucić samego siebie.\nZrób to sam, jeśli aż tak mnie nie lubisz 😢"));
                return;
            }

            if (!string.IsNullOrWhiteSpace(_config["log:kick:enable"]) && bool.Parse(_config["log:kick:enable"]))
            {
                var channel = Context.Guild.GetTextChannel(ulong.Parse(_config["log:kick:channel"]));
                
                if (channel == null)
                {
                    await ReplyAsync("", false, Embeds.Error(
                        "Nie odnalazłem kanału do wysłania logów moderacyjnych (a są one włączone)."));
                }
                else
                {
                    await channel.SendMessageAsync("", false, Embeds.ModLog(
                        ModerativeAction.Kick, 
                        $"{Context.Message.Author} / {Context.Message.Author.Mention}", 
                        $"{user} / {user.Mention}", 
                        reason)
                    );
                }
            }
            
            // Message the user that they were kicked the fuck out.
            await user.GetOrCreateDMChannelAsync();
            await user.SendMessageAsync("", false,
                Embeds.DirectPunished(ModerativeAction.Kick, Context.Message.Author.ToString(), reason));
            
            try
            {
                // Try to kick the user, and if we fail, notify about it.
                await user.KickAsync();
            }
            catch (HttpException exception)
            {
                // Yeah we fucked up.
                await ReplyAsync("", false, Embeds.Error(
                    $"Nie mogłem wyrzucić tego użytkownika z powodu błędu wewnętrznego.\n{exception.Message}"));
                return;
            }
            
            // And last but not least, confirm the operation.
            var confirmation = await ReplyAsync("", false, 
                Embeds.Ok($"Wyrzuciłem {user} / {user.Mention} z tego serwera.\nPowód: {reason}"));
            
            await Task.Delay(2750);
            await confirmation.DeleteAsync();
        }
        
        [Command("ban")]
        [Summary("Banuje użytkownika na serwerze.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        // ReSharper disable once UnusedMember.Global
        public async Task BanAsync(SocketGuildUser user, [Remainder] string reason = "Nie podano.")
        {
            if (user == Context.User)
            {
                // We can't ban the invoker, because that wouldn't make much sense.
                await ReplyAsync("", false, Embeds.Error(
                    "Nie możesz zbanować samego siebie."));
                return;
            }
            
            if (user.Hierarchy >= ((SocketGuildUser) Context.User).Hierarchy)
            {
                // We can't ban someone who's more privileged than command invoker.
                await ReplyAsync("", false, Embeds.Error(
                    "Nie możesz zbanować osoby o wyższym ani równym poziomie uprawnień."));
                return;
            }

            if (user.Id == Context.Client.CurrentUser.Id)
            {
                // We can't ban the bot account we're operating on - again, what's the sense in that?
                await ReplyAsync("", false, Embeds.Error(
                    "Umm... Nie mogę zbanować samego siebie.\nZrób to sam, jeśli aż tak mnie nie lubisz 😢"));
                return;
            }

            if (!string.IsNullOrWhiteSpace(_config["log:ban:enable"]) && bool.Parse(_config["log:ban:enable"]))
            {
                var channel = Context.Guild.GetTextChannel(ulong.Parse(_config["log:ban:channel"]));
                
                if (channel == null)
                {
                    await ReplyAsync("", false, Embeds.Error(
                        "Nie odnalazłem kanału do wysłania logów moderacyjnych (a są one włączone)."));
                }
                else
                {
                    await channel.SendMessageAsync("", false, Embeds.ModLog(
                        ModerativeAction.Ban, 
                        $"{Context.Message.Author} / {Context.Message.Author.Mention}", 
                        $"{user} / {user.Mention}", 
                        reason)
                    );
                }
            }
            
            // Message the user that they were fucking banned.
            await user.GetOrCreateDMChannelAsync();
            await user.SendMessageAsync("", false,
                Embeds.DirectPunished(ModerativeAction.Ban, Context.Message.Author.ToString(), reason));
            
            try
            {
                // Try to ban the user, and if we fail, notify about it.
                await user.BanAsync(7, $"{Context.Message.Author} | {reason}");
            }
            catch (HttpException exception)
            {
                // Yeah we fucked up.
                await ReplyAsync("", false, Embeds.Error(
                    $"Nie mogłem zbanować tego użytkownika z powodu błędu wewnętrznego.\n{exception.Message}"));
                return;
            }
            
            // And last but not least, confirm the operation.
            var confirmation = await ReplyAsync("", false, 
                Embeds.Ok($"Zbanowałem {user} / {user.Mention} na tym serwerze.\nPowód: {reason}"));
            
            await Task.Delay(2750);
            await confirmation.DeleteAsync();
        }
    }
}
