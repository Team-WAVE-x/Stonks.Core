using Discord;
using Discord.Commands;
using Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Nsfw
{
    public class Gelbooru : ModuleBase<SocketCommandContext>
    {
        [Command("겔부루")]
        public async Task GelbooruAsync([Remainder] string tags = null)
        {

        }
    }
}