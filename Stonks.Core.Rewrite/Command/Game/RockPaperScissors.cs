using Discord.Commands;
using Stonks.Core.Rewrite.Precondition;
using Stonks.Core.Rewrite.Service;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Game
{
    public class RockPaperScissors : ModuleBase<SocketCommandContext>
    {
        private readonly SqlService _sql;

        public RockPaperScissors(SqlService sql)
        {
            _sql = sql;
        }

        [Command("가위바위보")]
        [Cooldown(10)]
        public async Task RockPaperScissorsAsync([Remainder] string coin = null)
        {

        }
    }
}