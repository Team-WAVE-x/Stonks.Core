using Discord.Commands;
using Stonks.Core.Rewrite.Service;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Game
{
    public class Slotmachine : ModuleBase<SocketCommandContext>
    {
        private readonly SqlService _sql;

        public Slotmachine(SqlService sql)
        {
            _sql = sql;
        }

        [Command("슬롯머신")]
        public async Task SlotmachineAsync([Remainder] string coin = "")
        {
            var user = _sql.GetUser(Context.Guild.Id, Context.User.Id);

            if (string.IsNullOrWhiteSpace(coin))
            {
                await Context.Channel.SendMessageAsync("❌ 배팅할 금액을 입력하여 주세요.");
                return;
            }
            else if (!coin.All(char.IsDigit))
            {
                await Context.Channel.SendMessageAsync("❌ 배팅 금액은 반드시 소수가 아닌 양수이여야 합니다.");
                return;
            }
            else if (coin == "0" || Convert.ToUInt64(coin) < 0 || (Convert.ToDecimal(coin) % 1) > 0)
            {
                await Context.Channel.SendMessageAsync("❌ 배팅 금액은 반드시 1 이상의 정수여야 합니다.");
                return;
            }
            else if (user.Coin < Convert.ToUInt64(coin))
            {
                await Context.Channel.SendMessageAsync("❌ 돈이 부족합니다.");
                return;
            }
        }
    }
}