using Discord.Commands;
using Stonks.Core.Rewrite.Class;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Game
{
    public class Pay : ModuleBase<SocketCommandContext>
    {
        [Command("용돈")]
        [Cooldown(60)]
        public async Task HelpAsync()
        {
            await ReplyAsync("Test");
        }
    }
}