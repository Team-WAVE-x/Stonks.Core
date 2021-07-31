using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Rest;

using Stonks.Core.Module;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stonks.Core.Command
{
    public class GeneralCommand : InteractiveBase<SocketCommandContext>
    {
        [Command("핑", RunMode = RunMode.Async)]
        [Summary("서버와의 연결 지연시간을 확인합니다.")]
        public async Task PingAsync()
        {
            RestUserMessage message = await Context.Channel.SendMessageAsync($"Pinging...");
            TimeSpan latency = DateTime.Now - message.Timestamp;
            Color pingColor = new Color();

            if (latency.TotalMilliseconds < 50)
                pingColor = Color.Green;
            else if (latency.TotalMilliseconds < 100)
                pingColor = Color.Orange;
            else if (latency.TotalMilliseconds < 200)
                pingColor = Color.Red;

            EmbedBuilder builder = new EmbedBuilder()
            {
                Title = "🏓 Pong!",
                Color = pingColor,
                Fields = new List<EmbedFieldBuilder> 
                {
                    new EmbedFieldBuilder{ Name = "Gateway Ping", Value = $"`{Context.Client.Latency}ms`" },
                    new EmbedFieldBuilder{ Name = "Client Ping", Value = $"`{latency.TotalMilliseconds}ms`" }
                }
            };

            await message.ModifyAsync(msg => {
                msg.Content = null;
                msg.Embed = builder.Build();
            });
        }

        //이거 빨리 고쳐야함
        [Command("도움", RunMode = RunMode.Async)]
        [Alias("도움말")]
        [Summary("이 메시지를 표시합니다.")]
        public async Task HelpAsync()
        {
            //변수 설정
            const int MIN_PAGE = 0;
            const int MAX_PAGE = 4;

            int page = 0;

            EmbedBuilder[] builders = new EmbedBuilder[MAX_PAGE];
            List<CommandInfo> commands = Program.Commands.Commands.ToList();

            //builders 변수 초기화
            for (int i = 0; i < MAX_PAGE; i++)
            {
                builders[i] = new EmbedBuilder();
            }

            //게임 명령어 임베드
            builders[0].WithTitle("🎮 놀이 명령어");
            builders[0].WithColor(Color.Red);
            builders[0].WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.Client.GetUser(Program.Setting.DeveloperID).GetAvatarUrl(ImageFormat.Png, 128),
                Text = $"{Context.Client.GetUser(Program.Setting.DeveloperID).Username}#{Context.Client.GetUser(Program.Setting.DeveloperID).Discriminator} 제작"
            });
            builders[0].WithTimestamp(DateTimeOffset.Now);

            //기본 명령어 임베드
            builders[1].WithTitle("📄 기본 명령어");
            builders[1].WithColor(Color.Orange);
            builders[1].WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.Client.GetUser(Program.Setting.DeveloperID).GetAvatarUrl(ImageFormat.Png, 128),
                Text = $"{Context.Client.GetUser(Program.Setting.DeveloperID).Username}#{Context.Client.GetUser(Program.Setting.DeveloperID).Discriminator} 제작"
            });
            builders[1].WithTimestamp(DateTimeOffset.Now);

            //NSFW 명령어 임베드
            builders[2].WithTitle("🔞 NSFW 명령어");
            builders[2].WithColor(Color.Green);
            builders[2].WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.Client.GetUser(Program.Setting.DeveloperID).GetAvatarUrl(ImageFormat.Png, 128),
                Text = $"{Context.Client.GetUser(Program.Setting.DeveloperID).Username}#{Context.Client.GetUser(Program.Setting.DeveloperID).Discriminator} 제작"
            });
            builders[2].WithTimestamp(DateTimeOffset.Now);

            //전적 명령어 임베드
            builders[3].WithTitle("📈 전적 명령어");
            builders[3].WithColor(Color.Blue);
            builders[3].WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.Client.GetUser(Program.Setting.DeveloperID).GetAvatarUrl(ImageFormat.Png, 128),
                Text = $"{Context.Client.GetUser(Program.Setting.DeveloperID).Username}#{Context.Client.GetUser(Program.Setting.DeveloperID).Discriminator} 제작"
            });
            builders[3].WithTimestamp(DateTimeOffset.Now);

            //명령어 가져오기
            foreach (CommandInfo command in commands)
            {
                if (command.Module.Name != "AdminCommand")
                {
                    foreach (var item in builders)
                    {
                        item.AddField($"{Program.Setting.Prefix}{command.Name}", command.Summary);
                    }
                }
            }

            //전송
            RestUserMessage message = await Context.Channel.SendMessageAsync(embed: builders[0].Build());

            //대리자
            Action BackAction = async delegate
            {
                page--;

                if (page == -1)
                {
                    page = MIN_PAGE;
                }

                await message.ModifyAsync(msg => msg.Embed = builders[page].Build());
            };

            Action ForwardAction = async delegate
            {
                page++;

                if (page == MAX_PAGE)
                {
                    page = 1;
                }

                await message.ModifyAsync(msg => msg.Embed = builders[page].Build());
            };

            ReactMessageModule.CreateReactMessage(
                msg: message,
                emoji: new List<IEmote> { new Emoji("⬅️"), new Emoji("➡️") },
                action: new List<Action> { BackAction, ForwardAction },
                timeSpan: TimeSpan.FromMinutes(1),
                userId: Context.Message.Author.Id,
                guildId: Context.Guild.Id
            );
        }
    }
}