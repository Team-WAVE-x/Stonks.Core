using Discord;
using Discord.Commands;
using Stonks.Core.Rewrite.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.General
{
    public class React : ModuleBase<SocketCommandContext>
    {
        private readonly ReactService _react;

        public React(ReactService react)
        {
            _react = react;
        }

        [Command("react")]
        public async Task ReactAsync()
        {
            var dictionary = new Dictionary<IEmote, Action>
            {
                { new Emoji("⏭️"), async delegate { await Context.Channel.SendMessageAsync("I was Sent from Delegate!");  } }
            };
            var message = await Context.Channel.SendMessageAsync("Hello, World!");

            _react.AddReactionMessage(message, Context.User.Id, Context.Guild.Id, dictionary, TimeSpan.FromSeconds(10));
        }
    }
}
