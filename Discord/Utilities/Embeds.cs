using System;

namespace Discord.Utilities
{
    public static class Embeds
    {
        public static Embed Error(string message)
        {
            return new EmbedBuilder()
            {
                Color = new Color(183, 28, 28),
                Title = "Błąd",
                Description = message
            }.Build();
        }
        
        public static Embed Ok(string message)
        {
            return new EmbedBuilder()
            {
                Color = new Color(51, 105, 30),
                Title = "OK",
                Description = message
            }.Build();
        }

        public static Embed ModLog(ModerativeAction action, string invoker, string victim, string reason = null)
        {
            var embedBuilder = new EmbedBuilder()
                .WithDescription("Akcja moderatorska")
                .AddField("Wykonujący", invoker, true)
                .AddField("Cel", victim, true)
                .AddField("Powód", !string.IsNullOrWhiteSpace(reason) ? reason : "Brak dla akcji.")
                .WithCurrentTimestamp();

            switch (action)
            {
                case ModerativeAction.Prune:
                    embedBuilder.WithAuthor("Usunięcie wiadomości",
                        "https://img.icons8.com/cotton/96/000000/delete-message.png");
                    break;
                case ModerativeAction.Kick:
                    embedBuilder.WithAuthor("Wyrzucenie z serwera",
                        "https://img.icons8.com/cotton/500/000000/minus.png");
                    break;
                case ModerativeAction.Ban:
                    embedBuilder.WithAuthor("Zbanowanie",
                        "https://img.icons8.com/cotton/96/000000/delete-shield.png");
                    break;
                default:
                    embedBuilder.WithTitle("Nierozpoznana akcja");
                    break;
            }

            return embedBuilder.Build();
        }

        public static Embed DirectPunished(ModerativeAction action, string invoker, string reason)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Komunikat");
                
            var grammaticAction = "ukarany(a) na serwerze";

            if (action == ModerativeAction.Kick)
                grammaticAction = "wyrzucony(a) z serwera";
            else if (action == ModerativeAction.Ban) 
                grammaticAction = "zbanowany(a) na serwerze";

            embed.WithDescription($"Zostałeś(aś) {grammaticAction} Strefa Escape From Tarkov.");
            embed.AddField("Wykonujący", invoker, true);
            embed.AddField("Powód", reason, true);
            embed.WithFooter(
                "Możesz odwołać się od kary, wysyłając zgłoszenie o pomoc, używając komendy !ticket [treść].");
            embed.WithCurrentTimestamp();
            embed.WithColor(255, 255, 255);

            return embed.Build();
        }
    }
}