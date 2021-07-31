using BooruSharp.Booru;
using BooruSharp.Search.Post;

using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

using Stonks.Core.Module;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Command
{
    public class NsfwCommand : InteractiveBase<SocketCommandContext>
    {
        [Command("겔부루", RunMode = RunMode.Async)]
        [Summary("겔부루에서 랜덤한 이미지를 가져옵니다. (태그는 공백으로 구분합니다)")]
        public async Task GelbooruAsync([Remainder] string tags = "")
        {
            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await Context.Channel.SendMessageAsync("❌ 이 명령어는 NSFW 채널에서만 사용할 수 있습니다.");
                return;
            }

            BooruSharp.Search.Post.SearchResult result;

            try
            {
                Gelbooru booru = new Gelbooru();
                result = await booru.GetRandomPostAsync(tags.Split(' '));
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync("❌ 해당 태그가 존재하지 않습니다.");
                return;
            }

            EmbedBuilder builder = new EmbedBuilder
            {
                Title = "Gelbooru",
                Color = Color.Red,
                ImageUrl = result.FileUrl.AbsoluteUri,
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "아이디",
                        Value = $"`{result.ID}`"
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "생성일",
                        Value = $"`{result.Creation}`"
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "소스",
                        Value = $"`{(string.IsNullOrWhiteSpace(result.Source) ? "알 수 없음" : result.Source)}`"
                    }
                },
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                    Text = $"{Context.User.Username}"
                },
                Timestamp = DateTimeOffset.Now
            };

            var message = await Context.Channel.SendMessageAsync(embed: builder.Build());

            Action nextAction = async delegate
            {
                Gelbooru booru = new Gelbooru();
                result = await booru.GetRandomPostAsync(tags.Split(' '));

                EmbedBuilder builder = new EmbedBuilder
                {
                    Title = "Gelbooru",
                    Color = Color.Red,
                    ImageUrl = result.FileUrl.AbsoluteUri,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "아이디",
                            Value = result.ID
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "생성일",
                            Value = result.Creation
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "소스",
                            Value = $"`{(string.IsNullOrWhiteSpace(result.Source) ? "알 수 없음" : result.Source)}`"
                        }
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                        Text = $"{Context.User.Username}"
                    },
                    Timestamp = DateTimeOffset.Now
                };

                await message.ModifyAsync(msg => msg.Embed = builder.Build());
            };

            Action tagAction = async delegate
            {
                await Context.Channel.SendMessageAsync($"태그: `{string.Join(", ", result.Tags)}`");
            };

            Action closeAction = async delegate
            {
                ReactMessageModule.RemoveReactMessage(message.Id);
            };

            ReactMessageModule.CreateReactMessage(
                msg: message,
                emoji: new List<IEmote> { new Emoji("➡️"), new Emoji("🏷️"), new Emoji("❌") },
                action: new List<Action> { nextAction, tagAction, closeAction },
                timeSpan: TimeSpan.FromMinutes(5),
                userId: Context.Message.Author.Id,
                guildId: Context.Guild.Id,
                nsfw: true
            );
        }
    }
}
