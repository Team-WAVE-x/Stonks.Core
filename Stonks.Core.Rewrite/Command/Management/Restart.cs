using Discord;
using Discord.Commands;
using Stonks.Core.Rewrite.Class;
using Stonks.Core.Rewrite.Precondition;
using Stonks.Core.Rewrite.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.Management
{
    public class Restart : ModuleBase<SocketCommandContext>
    {
        private readonly ReactService _react;

        public Restart(ReactService react)
        {
            _react = react;
        }

        [Developer]
        [Command("재시작")]
        public async Task GelbooruAsync()
        {
            var message = await Context.Channel.SendMessageAsync("❓ 봇을 재시작할까요?");

            #region ReactMessage Delegate
            Action okAction = async delegate
            {
                var info = new System.Diagnostics.ProcessStartInfo(Environment.GetCommandLineArgs()[0]);
                System.Diagnostics.Process.Start(info);
            };

            Action cancelAction = async delegate
            {
                _react.RemoveReactionMessage(message.Id);
            };
            #endregion

            var dictionary = new Dictionary<IEmote, Action>
            {
                { new Emoji("✅"), okAction },
                { new Emoji("❌"), cancelAction }
            };

            _react.AddReactionMessage(message, Context.User.Id, Context.Guild.Id, dictionary, TimeSpan.FromSeconds(10));
        }
    }
}