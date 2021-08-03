using Stonks.Core.Rewrite.Service;

namespace Stonks.Core.Rewrite
{
    class Program
    {
        static void Main(string[] args) => new DiscordService().MainAsync().GetAwaiter().GetResult();
    }
}