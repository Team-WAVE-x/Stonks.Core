using Discord;
using System;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Service
{
    public class LoggingService
    {
        public Task OnClientLogReceived(LogMessage log)
        {
            Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "Client", log.Message ?? "Null");
            return Task.CompletedTask;
        }

        public Task OnCommandLogReceived(LogMessage log)
        {
            Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "Command", log.Message ?? "Null");
            return Task.CompletedTask;
        }
    }
}