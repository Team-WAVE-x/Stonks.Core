using Discord;
using Discord.Commands;
using Stonks.Core.Rewrite.Class;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        private readonly Setting _setting;

        public Help(Setting setting)
        {
            _setting = setting;
        }

        [Command("도움말")]
        public async Task HelpAsync()
        {
            var groups = _setting.CommandGroup;

            EmbedBuilder[] commandPages = new EmbedBuilder[groups.Length];
            commandPages.InitializeArray();

            for (int i = 0; i < groups.Length; i++)
            {
                commandPages[i].WithTitle(groups[i].Title);
                commandPages[i].WithDescription(groups[i].Description);
                commandPages[i].WithColor(new Color(Convert.ToUInt32(groups[i].Color, 16)));
                commandPages[i].WithFooter(new EmbedFooterBuilder()
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = Context.Client.CurrentUser.Username
                });
                commandPages[i].WithTimestamp(DateTimeOffset.Now);

                foreach (var item in groups[i].Commands)
                {
                    commandPages[i].AddField($"{_setting.Config.Prefix}{item.Name} {string.Join(", ", item.Args.Select(x => "{" + x + "}"))}", item.Description);
                }
            }

            foreach (var item in commandPages)
            {
                await ReplyAsync(embed: item.Build());
            }
        }
    }
}