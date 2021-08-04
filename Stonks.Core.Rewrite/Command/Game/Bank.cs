using Discord;
using Discord.Commands;

using LazyCache;
 
using Stonks.Core.Rewrite.Class;
using Stonks.Core.Rewrite.Service;

using System;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Game
{
    public class Bank : ModuleBase<SocketCommandContext>
    {
        private readonly SqlService _sql;

        public Bank(SqlService sql)
        {
            _sql = sql;
        }

        [Command("은행")]
        public async Task BankAsync()
        {
            var builder = new ComponentBuilder().WithButton("Hello!", customId: "id_1", ButtonStyle.Primary, row: 0);
            await Context.Channel.SendMessageAsync("Test buttons!", component: builder.Build());
        }
    }
}