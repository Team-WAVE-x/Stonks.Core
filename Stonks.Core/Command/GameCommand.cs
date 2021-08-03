using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;

using Stonks.Core.Class;
using Stonks.Core.Module;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Command
{
    public class GameCommand : InteractiveBase<SocketCommandContext>
    {
        [Command("용돈", RunMode = RunMode.Async)]
        public async Task CashAsync()
        {
            async void giveMoney()
            {
                User user = new User(Context.Guild.Id, Context.User.Id);
                Random rd = new Random();
                ulong value = (ulong)rd.Next(100, 1000);
                user.AddMoney(value);
                await Context.Channel.SendMessageAsync($"{value} 코인을 받았습니다.");
            }

            if (Program.StackCooldownTarget.Contains(Context.User as SocketGuildUser))
            {
                if (Program.StackCooldownTimer[Program.StackCooldownTarget.IndexOf(Context.Message.Author as SocketGuildUser)].AddMinutes(1) >= DateTimeOffset.Now)
                {
                    int secondsLeft = (int)(Program.StackCooldownTimer[Program.StackCooldownTarget.IndexOf(Context.Message.Author as SocketGuildUser)].AddMinutes(1) - DateTimeOffset.Now).TotalSeconds;
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}>님, 용돈을 다시 받을려면 {secondsLeft}초 기다려야 해요!");
                }
                else
                {
                    giveMoney();
                    Program.StackCooldownTimer[Program.StackCooldownTarget.IndexOf(Context.Message.Author as SocketGuildUser)] = DateTimeOffset.Now;
                }
            }
            else
            {
                giveMoney();
                Program.StackCooldownTarget.Add(Context.User as SocketGuildUser);
                Program.StackCooldownTimer.Add(DateTimeOffset.Now);
            }
        }

        [Command("내돈", RunMode = RunMode.Async)]
        public async Task MoneyAsync()
        {
            User user = new User(Context.Guild.Id, Context.User.Id);

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("💰 통장");
            builder.WithDescription($"당신의 통장에는 `{string.Format("{0:n0}", user.Money)}` 코인이 있습니다.");
            builder.WithColor(Color.Blue);
            builder.WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                Text = Context.User.Username
            });
            builder.WithTimestamp(DateTimeOffset.Now);

            await Context.Channel.SendMessageAsync(string.Empty, false, builder.Build());
        }

        [Command("랭킹", RunMode = RunMode.Async)]
        public async Task RankingAsync()
        {
            RestUserMessage message = await Context.Channel.SendMessageAsync("🧮 계산중...");
            List<User> users = GameModule.GetRanking(Context.Guild.Id, 20);

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("🏆 랭킹");
            builder.WithColor(Color.LightOrange);
            builder.WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                Text = Context.User.Username
            });
            builder.WithTimestamp(DateTimeOffset.Now);

            if (users.Count == 0)
            {
                builder.WithDescription("❌ 데이터가 존재하지 않습니다.");
            }
            else
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var user = await Context.Client.Rest.GetUserAsync(users[i].UserId);
                    builder.AddField($"{i + 1}등", $"{user.Username}#{user.Discriminator} - {string.Format("{0:n0}", users[i].Money)} 코인");
                }
            }

            await message.ModifyAsync(msg => { msg.Content = string.Empty; msg.Embed = builder.Build(); });
        }

        [Command("끝말잇기 랭킹", RunMode = RunMode.Async)]
        public async Task RoundRankingAsync()
        {
            RestUserMessage message = await Context.Channel.SendMessageAsync("🧮 계산중...");

            List<User> users = GameModule.GetRoundRanking(Context.Guild.Id, 20);

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("🏆 끝말잇기 랭킹");
            builder.WithColor(Color.LightOrange);
            builder.WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                Text = Context.User.Username
            });
            builder.WithTimestamp(DateTimeOffset.Now);

            if (users.Count == 0)
            {
                builder.WithDescription("❌ 데이터가 존재하지 않습니다.");
            }
            else
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var user = await Context.Client.Rest.GetUserAsync(users[i].UserId);
                    builder.AddField($"{i + 1}등", $"{user.Username}#{user.Discriminator} - {string.Format("{0:n0}", users[i].Round)} 라운드");
                }
            }

            await message.ModifyAsync(msg => { msg.Content = string.Empty; msg.Embed = builder.Build(); });
        }

        [Command("슬롯머신", RunMode = RunMode.Async)]
        public async Task SlotMachineAsync([Remainder] string money = "")
        {
            EmbedBuilder builder = new EmbedBuilder();
            User user = new User(Context.Guild.Id, Context.User.Id);

            //게임 참여 조건 확인
            if (string.IsNullOrWhiteSpace(money))
            {
                await Context.Channel.SendMessageAsync("❌ 배팅할 금액을 입력하여 주세요.");
                return;
            }
            else if (!money.All(char.IsDigit))
            {
                await Context.Channel.SendMessageAsync("❌ 배팅 금액은 반드시 소수가 아닌 양수이여야 합니다.");
                return;
            }
            else if (money == "0" || Convert.ToUInt64(money) < 0 || (Convert.ToDecimal(money) % 1) > 0)
            {
                await Context.Channel.SendMessageAsync("❌ 배팅 금액은 반드시 1 이상의 정수여야 합니다.");
                return;
            }
            else if (user.Money < Convert.ToUInt64(money))
            {
                await Context.Channel.SendMessageAsync("❌ 돈이 부족합니다.");
                return;
            }

            //게임중 리스트에 유저 추가
            Program.GamingUserList.Add(Context.Message.Author.Id);

            //임베드 기본 요소 추가
            builder.WithTitle("🎲 슬롯머신");
            builder.WithColor(Color.Orange);
            builder.WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                Text = Context.User.Username
            });
            builder.WithTimestamp(DateTimeOffset.Now);

            //슬롯머신 아이템 리스트 초기화
            List<string> slotMachineItems = new List<string>();
            Random rd = new Random();

            for (int i = 0; i < 3; i++)
            {
                int value = rd.Next(1, 101);

                if (value <= 30)                         //1 ~ 30
                {
                    slotMachineItems.Add(":melon:");
                }
                else if (value > 30 && value <= 60)      //31 ~ 60
                {
                    slotMachineItems.Add(":cherries:");
                }
                else if (value > 60 && value <= 90)      //61 ~ 90
                {
                    slotMachineItems.Add(":lemon:");
                }
                else if (value > 90 && value <= 95)      //91 ~ 95
                {
                    slotMachineItems.Add(":star:");
                }
                else if (value > 95 && value <= 99)      //96 ~ 99
                {
                    slotMachineItems.Add(":bell:");
                }
                else if (value == 100)                   //100
                {
                    slotMachineItems.Add(":seven:");
                }
            }

            //초기 상태 설정
            builder.WithDescription(slotMachineItems[0]);

            //메시지 객체 저장
            RestUserMessage message = await Context.Channel.SendMessageAsync(string.Empty, false, builder.Build());

            //유저에게 리스트 보여주는 연출
            StringBuilder stringBuilder = new StringBuilder(slotMachineItems[0]);
            for (int i = 1; i < 3; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

                stringBuilder.Append(slotMachineItems[i]);

                builder.WithDescription(stringBuilder.ToString());
                await message.ModifyAsync(msg => msg.Embed = builder.Build());
            }

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            int multiply = 0;

            if (slotMachineItems[0] == ":seven:" && slotMachineItems[1] == ":seven:" && slotMachineItems[2] == ":seven:")
            {
                multiply = 10;
            }
            else if (slotMachineItems[0] == "⭐" && slotMachineItems[1] == "⭐" && slotMachineItems[2] == "⭐")
            {
                multiply = 7;
            }
            else if (slotMachineItems[0] == "🔔" && slotMachineItems[1] == "🔔" && slotMachineItems[2] == "🔔")
            {
                multiply = 5;
            }
            else if (slotMachineItems[0] == slotMachineItems[1] && slotMachineItems[1] == slotMachineItems[2])
            {
                multiply = 3;
            }
            else if (slotMachineItems[0] == slotMachineItems[2])
            {
                multiply = 2;
            }
            else
            {
                multiply = 0;
            }

            builder.WithColor(Color.Teal);

            if (multiply == 0)
            {
                builder.WithTitle($"💸 꽝..");
                builder.WithColor(Color.Red);
                builder.WithDescription($"슬롯머신에서 `{string.Format("{0:n0}", Convert.ToUInt64(money))}` 코인을 잃었습니다...");

                user.SubMoney(Convert.ToUInt64(money));
            }
            else
            {
                builder.WithTitle($"{slotMachineItems[0]}{slotMachineItems[1]}{slotMachineItems[2]} {multiply}배!");
                builder.WithColor(Color.Green);
                builder.WithDescription($"슬롯머신에서 잭팟이 나와 `{string.Format("{0:n0}", Convert.ToUInt64(money) * Convert.ToUInt64(multiply))}` 코인을 얻었습니다!");

                user.AddMoney(Convert.ToUInt64(money) * Convert.ToUInt64(multiply));
            }

            //게임중 리스트에서 제거
            Program.GamingUserList.Remove(Context.Message.Author.Id);

            //메시지 수정
            await message.ModifyAsync(msg => msg.Embed = builder.Build());
        }

        [Command("끝말잇기", RunMode = RunMode.Async)]
        public async Task WordAsync()
        {
            //각종 필요한 변수를 정의함
            int round = 0;
            int wrongCount = 0;
            string word = GameModule.GetRandomWords();
            List<string> newWord = new List<string>();
            List<string> usedWords = new List<string>();
            EmbedBuilder builder = new EmbedBuilder();
            User user = new User(Context.Guild.Id, Context.User.Id);

            await ReplyAsync("끝말잇기 시작!");
            await ReplyAsync($"{word}!"); //아무 단어나 가져와서 메시지를 보냄
            usedWords.Add(word); //그리고 사용한 단어 리스트에 집어넣음
            Program.GamingUserList.Add(Context.Message.Author.Id); //게임중인 유저를 봇이 무시하도록 리스트에 집어넣음

            Stopwatch sw = new Stopwatch(); //스탑워치를 정의함

            //임베드 기본 내용 초기화하기
            builder.WithTitle("📋 끝말잇기");
            builder.WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                Text = Context.User.Username
            });
            builder.WithTimestamp(DateTimeOffset.Now);

            Start: //goto에 사용할 레이블
            SocketMessage response = await NextMessageAsync(true, true, TimeSpan.FromMilliseconds(10000 - sw.ElapsedMilliseconds)); //10초 이내에 답을 말하지 않으면 null을 저장하고 아니라면 값을 저장함
            sw.Start(); //스톱워치를 시작함

            //라운드 수 재설정
            builder.Fields.Clear();
            builder.AddField("버틴 라운드 수", $"{round} 라운드");

            if (response == null) //답변이 없을 경우 (게임 오버)
            {
                builder.WithColor(Color.Red);
                builder.WithDescription($"답변 시간을 초과하셔서 게임에서 패배했습니다..");
                await ReplyAsync(string.Empty, false, builder.Build());
            }
            else if (wrongCount > 2) //틀린 횟수가 2 초과시 틀림 (게임 오버)
            {
                builder.WithColor(Color.Red);
                builder.WithDescription($"답변 가능 횟수를 초과하셔서 게임에서 패배했습니다..");
                await ReplyAsync(string.Empty, false, builder.Build());
            }
            else if (word[word.Length - 1] != response.Content[0]) //앞글자가 같지 않음
            {
                wrongCount++;

                await ReplyAsync("❌ 앞 글자가 맞지 않습니다!");
                goto Start;
            }
            else if (!GameModule.IsWordExist(response.Content) || (response.Content.Length == 1)) //없는 단어 사용
            {
                wrongCount++;

                await ReplyAsync("❌ 존재하지 않는 단어입니다!");
                goto Start;
            }
            else if (usedWords.Contains(response.Content)) //이미 사용한 단어
            {
                wrongCount++;

                await ReplyAsync("❌ 이미 사용한 단어입니다!");
                goto Start;
            }
            else
            {
                newWord = GameModule.GetStartWords(response.Content[response.Content.Length - 1].ToString()); //특정 글자로 시작하는 단어 리스트를 가져옴

                //그런다음 사용한 단어를 "특정 글자로 시작하는 단어 리스트" 에서 뺌
                foreach (string item in usedWords)
                {
                    if (!item.StartsWith(response.Content[response.Content.Length - 1].ToString()))
                    {
                        newWord.Remove(item);
                    }
                }

                Random rnd = new Random();
                int r = rnd.Next(newWord.Count);
                newWord.Sort();

                word = newWord[r]; //랜덤한 단어를 가져옴

                if (string.IsNullOrWhiteSpace(word)) //랜덤한 단어를 가져올게 없는 경우 게임 승리
                {
                    builder.WithColor(Color.Green);
                    builder.WithDescription($"게임에서 승리하셨습니다!");
                    builder.AddField("상금", $"{round * 10} 코인");

                    user.AddMoney(Convert.ToUInt64(round * 10));

                    await ReplyAsync(string.Empty, false, builder.Build());
                }
                else //랜덤한 사용하지 않은 단어가 있다면
                {
                    await ReplyAsync($"{word}!");

                    round++;

                    usedWords.Add(word);
                    usedWords.Add(response.Content);

                    sw.Stop();
                    sw.Reset();

                    goto Start;
                }
            }

            if (user.Round < round)
            {
                user.SetScore(round);
            }

            Program.GamingUserList.Remove(Context.Message.Author.Id);
            sw.Stop();
            sw.Reset();
        }

        [Command("업다운", RunMode = RunMode.Async)]
        public async Task UpDownGameAsync()
        {
            Random rd = new Random();
            int num = rd.Next(1, 100);

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("↕️ 업다운 게임");
            builder.WithDescription("수의 범위는 1 ~ 99 입니다.");
            builder.WithFooter(new EmbedFooterBuilder
            {
                IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                Text = Context.User.Username
            });
            builder.WithTimestamp(DateTimeOffset.Now);

            await Context.Channel.SendMessageAsync(string.Empty, false, builder.Build());

            Start:
            SocketMessage response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(10));

            if (response.Content == null)
            {
                await Context.Channel.SendMessageAsync("❌ 시간 초과!");
            }
            else if (!response.Content.All(char.IsDigit))
            {
                await Context.Channel.SendMessageAsync("❌ 수는 반드시 소수가 아닌 양수이여야 합니다.");
            }
            else if (response.Content == "0" || Convert.ToUInt64(response.Content) < 0 || (Convert.ToDecimal(response.Content) % 1) > 0 || Convert.ToUInt64(response.Content) > 100)
            {
                await Context.Channel.SendMessageAsync("❌ 수는 반드시 1 이상 100 이하의 정수여야 합니다.");
            }
            else if (response.Content == num.ToString())
            {
                builder.WithTitle("↕️ 업다운 게임");
                builder.WithColor(Color.Green);
                builder.WithDescription($"🎉 게임에서 승리하셨습니다!");
                builder.WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                    Text = Context.User.Username
                });
                builder.WithTimestamp(DateTimeOffset.Now);

                await ReplyAsync(string.Empty, false, builder.Build());
            }
            else if (Convert.ToInt32(response.Content) > num)
            {
                await Context.Channel.SendMessageAsync("🔽 다운!");
                goto Start;
            }
            else if (Convert.ToInt32(response.Content) < num)
            {
                await Context.Channel.SendMessageAsync("🔼 업!");
                goto Start;
            }
        }

        [Command("고양이", RunMode = RunMode.Async)]
        public async Task CatAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            using (WebClient client = new WebClient())
            {
                builder.WithTitle("🐱 고양이");
                builder.WithImageUrl(JObject.Parse(client.DownloadString("http://aws.random.cat/meow")).SelectToken("file").ToString());
                builder.WithColor(Color.LightOrange);
                builder.WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                    Text = Context.User.Username
                });
                builder.WithTimestamp(DateTimeOffset.Now);
            }

            RestUserMessage message = await Context.Channel.SendMessageAsync(embed: builder.Build());

            Action action = async delegate
            {
                using (WebClient client = new WebClient())
                {
                    builder.WithTitle("🐱 고양이");
                    builder.WithImageUrl(JObject.Parse(client.DownloadString("http://aws.random.cat/meow")).SelectToken("file").ToString());
                    builder.WithColor(Color.LightOrange);
                    builder.WithFooter(new EmbedFooterBuilder
                    {
                        IconUrl = Context.User.GetAvatarUrl(ImageFormat.Png, 128),
                        Text = Context.User.Username
                    });
                    builder.WithTimestamp(DateTimeOffset.Now);
                }

                await message.ModifyAsync(msg => msg.Embed = builder.Build());
            };

            ReactMessageModule.CreateReactMessage(
                msg: message,
                emoji: new List<IEmote> { new Emoji("➡️") },
                action: new List<Action> { action },
                timeSpan: TimeSpan.FromMinutes(1),
                userId: Context.Message.Author.Id,
                guildId: Context.Guild.Id
            );
        }
    }
}