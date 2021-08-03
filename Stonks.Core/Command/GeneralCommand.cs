using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Rest;

using Stonks.Core.Class;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stonks.Core.Command
{
    public class GeneralCommand : InteractiveBase<SocketCommandContext>
    {
        [Command("핑", RunMode = RunMode.Async)]
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

        [Command("도움말", RunMode = RunMode.Async)]
        public async Task HelpAsync()
        {
            var groups = Program.Setting.CommandGroup;

            EmbedBuilder commandGroup = new EmbedBuilder();
            EmbedBuilder[] commandPages = new EmbedBuilder[groups.Length];
            commandPages.InitializeArray();

            //메인 페이지
            commandGroup.WithTitle("📜 도움말");
            commandGroup.WithColor(Color.Teal);
            commandGroup.WithDescription("확인하고 싶은 명령어의 그룹을 선택해 주세요.");

            foreach (var item in groups)
            {
                commandGroup.AddField(item.Title, item.Description);
            }
        }
    }
}