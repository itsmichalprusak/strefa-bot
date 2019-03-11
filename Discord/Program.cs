using System.Threading.Tasks;

namespace Discord
{
    internal static class Program
    {
        public static async Task Main(string[] args)
            => await Startup.RunAsync(args);
    }
}
