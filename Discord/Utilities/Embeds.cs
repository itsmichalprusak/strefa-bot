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
    }
}