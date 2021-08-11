using Discord;
using Discord.Commands;
using Interactivity;
using Interactivity.Pagination;
using Stonks.Core.Rewrite.Class;
using Stonks.Core.Rewrite.Extension;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        private readonly InteractivityService _interactivity;
        private readonly Setting _setting;

        public Help(Setting setting, InteractivityService interactivity)
        {
            _setting = setting;
            _interactivity = interactivity;
        }

        [Command("도움말")]
        public async Task HelpAsync()
        {
            var groups = _setting.CommandGroup;
            var commandPages = new PageBuilder[groups.Length];
            commandPages.InitializeArray();

            for (int i = 0; i < groups.Length; i++)
            {
                commandPages[i].WithTitle(groups[i].Title);
                commandPages[i].WithDescription(groups[i].Description);
                commandPages[i].WithColor(new Color(Convert.ToUInt32(groups[i].Color, 16)));
                commandPages[i].WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                    Text = Context.User.Username

                });
                foreach (var item in groups[i].Commands)
                {
                    commandPages[i].AddField($"{_setting.Config.Prefix}{item.Name} {string.Join(", ", item.Args.Select(x => "{" + x + "}"))}", item.Description);
                }
            }

            var paginator = new StaticPaginatorBuilder()
                .WithUsers(Context.User)
                .WithPages(commandPages)
                .WithCancelledEmbed(new EmbedBuilder() { Title = "🛑 명령어가 취소되었습니다.", Color = Color.Red })
                .WithTimoutedEmbed(new EmbedBuilder() { Title = "🛑 대기 시간이 초과되었습니다.", Color = Color.Red })
                .WithDefaultEmotes()
                .Build();

            await _interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
        }
    }
}