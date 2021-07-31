using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Rest;

using Stonks.Core.Module;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stonks.Core.Command
{
    public class AdminCommand : InteractiveBase<SocketCommandContext>
    {
        [Command("재시작", RunMode = RunMode.Async)]
        public async Task RestartAsync()
        {
            if (Context.User.Id == Program.Setting.Config.DeveloperId)
            {
                EmbedBuilder builder = new EmbedBuilder
                {
                    Title = "🔄 재시작",
                    Description = "정말 재시작 하시겠습니까?",
                    Color = Color.Red,
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                        Text = $"{Context.User.Username}"
                    },
                    Timestamp = DateTimeOffset.Now
                };

                RestUserMessage message = await Context.Channel.SendMessageAsync(embed: builder.Build());

                Action OkAction = async delegate
                {
                    ReactMessageModule.RemoveReactMessage(message.Id);

                    builder = new EmbedBuilder
                    {
                        Title = "🔄 재시작",
                        Description = "봇이 재시작 중입니다. 이 작업은 시간이 걸릴 수 있습니다...",
                        Color = Color.Green,
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                            Text = $"{Context.User.Username}"
                        },
                        Timestamp = DateTimeOffset.Now
                    };

                    await message.RemoveAllReactionsAsync();
                    await message.ModifyAsync(msg => msg.Embed = builder.Build());

                    System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + System.AppDomain.CurrentDomain.FriendlyName);
                    Environment.Exit(0);
                };

                Action CancelAction = async delegate
                {
                    ReactMessageModule.RemoveReactMessage(message.Id);

                    await message.RemoveAllReactionsAsync();
                    await message.ModifyAsync(msg => { msg.Content = "❌ 작업이 취소되었습니다."; msg.Embed = null; });
                };

                ReactMessageModule.CreateReactMessage(
                    msg: message,
                    emoji: new List<IEmote> { new Emoji("✅"), new Emoji("❎") },
                    action: new List<Action> { OkAction, CancelAction },
                    timeSpan: TimeSpan.FromMinutes(1),
                    userId: Context.Message.Author.Id,
                    guildId: Context.Guild.Id
                );
            }
            else
            {
                await Context.Channel.SendMessageAsync("❌ 개발자만 사용할 수 있는 명령어입니다.");
            }
        }

        [Command("업타임", RunMode = RunMode.Async)]
        public async Task UptimeAsync()
        {
            if (Context.User.Id == Program.Setting.Config.DeveloperId)
            {
                TimeSpan uptime = TimeSpan.FromMilliseconds(Program.UptimeStopwatch.ElapsedMilliseconds);

                EmbedBuilder builder = new EmbedBuilder
                {
                    Title = "🕒 업타임",
                    Description = $"{uptime.Days}일 {uptime.Hours}시간 {uptime.Minutes}분 {uptime.Seconds}초",
                    Color = Color.Teal,
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                        Text = $"{Context.User.Username}"
                    },
                    Timestamp = DateTimeOffset.Now
                };

                await Context.Channel.SendMessageAsync(embed: builder.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("❌ 개발자만 사용할 수 있는 명령어입니다.");
            }
        }


        [Command("공지", RunMode = RunMode.Async)]
        public async Task NoticeAsync()
        {
            if (Context.User.Id == Program.Setting.Config.DeveloperId)
            {
                await Context.Channel.SendMessageAsync("공지의 내용을 입력해 주십시오.");
                var response = await NextMessageAsync(true, true, TimeSpan.FromMinutes(10));

                if (response != null)
                {
                    EmbedBuilder builder = new EmbedBuilder
                    {
                        Title = "🕒 업타임",
                        Description = $"정말 이 공지를 전송할까요?",
                        Color = Color.Red,
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                            Text = $"{Context.User.Username}"
                        },
                        Timestamp = DateTimeOffset.Now
                    };

                    var message = await Context.Channel.SendMessageAsync(embed: builder.Build());

                    Action OkAction = async delegate
                    {
                        ReactMessageModule.RemoveReactMessage(message.Id);

                        foreach (var item in Context.Client.Guilds)
                        {
                            EmbedBuilder builder = new EmbedBuilder
                            {
                                Title = "📌 공지",
                                Description = response.Content,
                                Color = Color.Green,
                                Footer = new EmbedFooterBuilder
                                {
                                    IconUrl = Context.Client.GetUser(Program.Setting.Config.DeveloperId).GetAvatarUrl(ImageFormat.Png, 128),
                                    Text = $"{Context.Client.GetUser(Program.Setting.Config.DeveloperId).Username}#{Context.Client.GetUser(Program.Setting.Config.DeveloperId).Discriminator}"
                                },
                                Timestamp = DateTimeOffset.Now
                            };

                            await item.SystemChannel.SendMessageAsync(embed: builder.Build());
                        }

                        await message.RemoveAllReactionsAsync();
                        await message.ModifyAsync(msg => { msg.Content = "✅ 공지가 전송되었습니다."; msg.Embed = null; });
                    };

                    Action CancelAction = async delegate
                    {
                        ReactMessageModule.RemoveReactMessage(message.Id);

                        await message.RemoveAllReactionsAsync();
                        await message.ModifyAsync(msg => { msg.Content = "❌ 작업이 취소되었습니다."; msg.Embed = null; });
                    };

                    ReactMessageModule.CreateReactMessage(
                        msg: message,
                        emoji: new List<IEmote> { new Emoji("✅"), new Emoji("❎") },
                        action: new List<Action> { OkAction, CancelAction },
                        timeSpan: TimeSpan.FromMinutes(1),
                        userId: Context.Message.Author.Id,
                        guildId: Context.Guild.Id
                    );
                }
                else
                {
                    await Context.Channel.SendMessageAsync("❌ 입력 시간이 초과되었습니다.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("❌ 개발자만 사용할 수 있는 명령어입니다.");
            }
        }
    }
}