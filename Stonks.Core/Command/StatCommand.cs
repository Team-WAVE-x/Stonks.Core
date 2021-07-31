using Discord.Addons.Interactive;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Stonks.Core.Module;
using Stonks.Core.Class.CSGO;

using Newtonsoft.Json;

using Discord;

namespace Stonks.Core.Command
{
    public class StatCommand : InteractiveBase<SocketCommandContext>
    {
        [Command("카스", RunMode = RunMode.Async)]
        [Summary("카운터스트라이크 : 글로벌 오펜시브 유저의 정보를 가져옵니다.")]
        public async Task CSAsync([Remainder] string id = "")
        {
            //https://tracker.gg/developers/docs/titles/csgo

            if (string.IsNullOrWhiteSpace(id))
            {
                await Context.Channel.SendMessageAsync("❌ 검색할 유저의 스팀 아이디 또는 유저 페이지 URL 을(를) 입력해 주세요!");
                return;
            }

            string userId = string.Empty;

            //유저 찾기
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("TRN-Api-Key", Program.Setting.Config.TrnToken);
                client.Headers.Add(HttpRequestHeader.Accept, "application/json");

                string json = client.DownloadString($"https://public-api.tracker.gg/v2/csgo/standard/search?platform=steam&query={id}");
                User user = JsonConvert.DeserializeObject<User>(json);

                try
                {
                    userId = user.Data[0].PlatformUserIdentifier;
                }
                catch (Exception)
                {
                    await Context.Channel.SendMessageAsync("❌ 해당 유저가 존재하지 않습니다. 올바른 스팀 아이디 또는 스팀 유저 페이지 URL 을(를) 입력했는지 확인해 주세요.");
                    return;
                }
            }

            //정보 불러오기
            using (WebClient client = new WebClient())
            {
                Stat stat = new Stat();

                client.Headers.Add("TRN-Api-Key", Program.Setting.Config.TrnToken);
                client.Headers.Add(HttpRequestHeader.Accept, "application/json");

                try
                {
                    string json = client.DownloadString($"https://public-api.tracker.gg/v2/csgo/standard/profile/steam/{userId}");
                    stat = JsonConvert.DeserializeObject<Stat>(json);
                }
                catch (Exception)
                {
                    await Context.Channel.SendMessageAsync("❌ 해당 유저가 존재하지 않습니다. 올바른 스팀 아이디를 입력했는지 확인해 주세요.");
                    return;
                }

                EmbedBuilder builder = new EmbedBuilder
                {
                    Title = $"<:2219_cs_logo:856055062072786954> {stat.Data.PlatformInfo.PlatformUserHandle}의 CS:GO 전적",
                    Color = Color.LighterGrey,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "플레이한 시간",
                            Value = $"{Math.Round(TimeSpan.FromSeconds(stat.Data.Segments[0].Stats["timePlayed"].Value).TotalHours, 1)} 시간",
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "킬",
                            Value = stat.Data.Segments[0].Stats["kills"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "데스",
                            Value = stat.Data.Segments[0].Stats["deaths"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Kill/Death",
                            Value = stat.Data.Segments[0].Stats["kd"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "데미지",
                            Value = stat.Data.Segments[0].Stats["damage"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "제압",
                            Value = stat.Data.Segments[0].Stats["dominations"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "발사된 총알",
                            Value = stat.Data.Segments[0].Stats["shotsFired"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "맞춘 총알",
                            Value = stat.Data.Segments[0].Stats["shotsHit"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "정확도",
                            Value = stat.Data.Segments[0].Stats["shotsAccuracy"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "처치한 스나이퍼의 수",
                            Value = stat.Data.Segments[0].Stats["snipersKilled"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "오버킬",
                            Value = stat.Data.Segments[0].Stats["dominationOverkills"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "복수",
                            Value = stat.Data.Segments[0].Stats["dominationRevenges"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "설치한 폭탄",
                            Value = stat.Data.Segments[0].Stats["bombsPlanted"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "해체한 폭탄",
                            Value = stat.Data.Segments[0].Stats["bombsDefused"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "획득한 돈",
                            Value = stat.Data.Segments[0].Stats["moneyEarned"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "구출한 인질",
                            Value = stat.Data.Segments[0].Stats["hostagesRescued"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "MVP",
                            Value = stat.Data.Segments[0].Stats["mvp"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "승리",
                            Value = stat.Data.Segments[0].Stats["wins"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "무승부",
                            Value = stat.Data.Segments[0].Stats["ties"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "패배",
                            Value = stat.Data.Segments[0].Stats["losses"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "플레이한 매치",
                            Value = stat.Data.Segments[0].Stats["matchesPlayed"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "플레이한 라운드",
                            Value = stat.Data.Segments[0].Stats["roundsPlayed"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "승률",
                            Value = stat.Data.Segments[0].Stats["wlPercentage"].DisplayValue,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "헤드샷 확률",
                            Value = stat.Data.Segments[0].Stats["headshotPct"].DisplayValue,
                            IsInline = true
                        },
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                        Text = $"{Context.User.Username}"
                    },
                    Timestamp = DateTimeOffset.Now
                };

                await Context.Channel.SendMessageAsync(embed: builder.Build());
            }
        }
    }
}
