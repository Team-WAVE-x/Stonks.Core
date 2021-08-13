using Discord.Commands;
using Stonks.Core.Rewrite.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Stonks.Core.Rewrite.Class;

namespace Stonks.Core.Rewrite.Command.Game
{
    public class Slotmachine : ModuleBase<SocketCommandContext>
    {
        private readonly SqlService _sql;

        public Slotmachine(SqlService sql)
        {
            _sql = sql;
        }

        [Command("슬롯머신", RunMode = RunMode.Sync)]
        public async Task SlotmachineAsync([Remainder] string coin = null)
        {
            //게임 가능한지 조건 확인
            var user = _sql.GetUser(Context.Guild.Id, Context.User.Id);

            if (string.IsNullOrWhiteSpace(coin))
            {
                await Context.Channel.SendMessageAsync("❌ 배팅할 코인을 입력하여 주세요.");
                return;
            }
            else if (!coin.All(char.IsDigit))
            {
                await Context.Channel.SendMessageAsync("❌ 배팅할 코인은 반드시 소수가 아닌 양수이여야 합니다.");
                return;
            }
            else if (Convert.ToUInt64(coin) < 0 || (Convert.ToDecimal(coin) % 1) > 0)
            {
                await Context.Channel.SendMessageAsync("❌ 배팅할 코인은 반드시 1 이상의 정수여야 합니다.");
                return;
            }
            else if (user.Coin < Convert.ToUInt64(coin))
            {
                await Context.Channel.SendMessageAsync("❌ 코인이 부족합니다.");
                return;
            }

            //게임 로직 시작
            var items = new List<SlotmachineUtility.Item>();

            for (int i = 1; i < 3; i++)
            {
                items.Add(SlotmachineUtility.RandomEnum());
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("🎲 슬롯머신");
            embed.WithDescription(SlotmachineUtility.EnumToEmoji(items[0]).ToString());
            embed.WithColor(Color.Orange);

            var message = await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}