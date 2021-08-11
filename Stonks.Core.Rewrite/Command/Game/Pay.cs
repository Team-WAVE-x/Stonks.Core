using Discord.Commands;
using Stonks.Core.Rewrite.Precondition;
using Stonks.Core.Rewrite.Service;
using System;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Game
{
    public class Pay : ModuleBase<SocketCommandContext>
    {
        private readonly SqlService _sql;

        public Pay(SqlService sql)
        {
            _sql = sql;
        }

        [Command("용돈")]
        [Cooldown(60)]
        public async Task HelpAsync()
        {
            Random random = new Random();
            int value = random.Next(1, 1000);

            _sql.AddUserCoin(Context.Guild.Id, Context.User.Id, value);
            await ReplyAsync($"💰 용돈으로 `{string.Format("{0:#,0}", value)}` 코인을 받았습니다!");
        }
    }
}