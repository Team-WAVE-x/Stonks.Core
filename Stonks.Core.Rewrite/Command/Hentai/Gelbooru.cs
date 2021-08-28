using Discord;
using Discord.Commands;
using Interactivity;
using Stonks.Core.Rewrite.Precondition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Hentai
{
    public class Gelbooru : ModuleBase<SocketCommandContext>
    {
        [Nsfw]
        [Command("겔부루")]
        public async Task GelbooruAsync([Remainder] string tags = null)
        {
            
        }
    }
}