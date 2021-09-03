using Discord.Commands;
using Stonks.Core.Rewrite.Class;
using Stonks.Core.Rewrite.Precondition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Management
{
    public class Restart : ModuleBase<SocketCommandContext>
    {
        [Developer]
        [Command("재시작")]
        public async Task GelbooruAsync()
        {
            await ReplyAsync("안녕 나는 개발자만 실행할 수 있는 명령어");
        }
    }
}