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
    }
}