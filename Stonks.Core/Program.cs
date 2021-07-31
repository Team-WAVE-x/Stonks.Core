using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Stonks.Core.Class;
using Stonks.Core.Module;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Timers;

namespace Stonks.Core
{
    public class Program
    {
        private Timer Timer;
        private IServiceProvider Services;
        public readonly static Setting Setting = SettingModule.GetSettingInfo();
        public readonly static CommandService Commands = new CommandService(new CommandServiceConfig { LogLevel = LogSeverity.Debug });
        public readonly static DiscordSocketClient Client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug });

        public static Stopwatch UptimeStopwatch = new Stopwatch();
        public static List<ulong> GamingUserList = new List<ulong>();
        public static List<DateTimeOffset> StackCooldownTimer = new List<DateTimeOffset>();
        public static List<SocketGuildUser> StackCooldownTarget = new List<SocketGuildUser>();

        private static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            Client.Log += OnClientLogReceived;
            Client.ReactionAdded += OnReactionAdded;
            Client.MessageReceived += OnClientMessage;
            Client.Connected += OnClientLoggedIn;

            Commands.Log += OnClientLogReceived;
            Commands.CommandExecuted += OnCommandExecuted;

            UptimeStopwatch.Start();

            Timer = new Timer
            {
                Interval = 600000
            };
            Timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            Timer.Start();

            await Client.LoginAsync(TokenType.Bot, Setting.Token);
            await Client.StartAsync();

            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();

            await Client.SetGameAsync($"{Setting.Prefix}도움말", null, ActivityType.Playing);
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private async Task OnClientLoggedIn()
        {
            #if !DEBUG
                TimerElapsed(null, null);
                await (Client.GetChannel(Setting.ErrorLogChannelID) as ISocketMessageChannel).SendMessageAsync($"✅ 봇이 성공적으로 디스코드 게이트웨이에 접속하였습니다. ({Client.Latency}ms)");
            #endif
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                var data = new { servers = Client.Guilds.Count };
                string json = JsonConvert.SerializeObject(data);

                wc.Headers.Add(HttpRequestHeader.Authorization, Setting.KoreanbotsToken);
                wc.Headers.Set(HttpRequestHeader.ContentType, "application/json");

                try
                {
                    wc.UploadString($"https://koreanbots.dev/api/v2/bots/{Setting.KoreanbotsID}/stats", json);
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        string response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "API", JObject.Parse(response).SelectToken("message"));
                    }
                }
            }
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            MemoryCache cache = MemoryCache.Default;

            if (message != null && reaction.User.IsSpecified && reaction.UserId != Client.CurrentUser.Id)
            {
                foreach (KeyValuePair<string, object> items in cache)
                {
                    ReactMessage reactMessage = items.Value as ReactMessage;

                    if (message.Id.ToString() == items.Key && reactMessage.messageEmote.Contains(reaction.Emote) && reactMessage.messageUserId == reaction.UserId && reaction.Channel.Id == originChannel.Id) //반응이 달린 메시지의 아이디가 캐싱되어 있다면
                    {
                        Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "React", $"{reaction.User.Value} Added React At {message.Id}");

                        for (int i = 0; i < reactMessage.messageEmote.Count; i++)
                        {
                            if (reactMessage.messageEmote[i].Name == reaction.Emote.Name)
                            {
                                reactMessage.messageAction[i]();
                            }
                        }

                        await message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    }
                }
            }
        }

        public async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess)
                return;

            EmbedBuilder builder = new EmbedBuilder
            {
                Title = "❌ 오류",
                Color = Color.Red,
                Description = $"`{result}`",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Client.CurrentUser.GetAvatarUrl(),
                    Text = Client.CurrentUser.Username
                },
                Timestamp = DateTimeOffset.Now
            };

            await (context.Client.GetChannelAsync(Setting.ErrorLogChannelID).Result as ISocketMessageChannel).SendMessageAsync(embed: builder.Build());
        }

        private async Task OnClientMessage(SocketMessage rawMessage)
        {
            int argPos = 0;

            if (!(rawMessage is SocketUserMessage message) || message.Source != MessageSource.User || rawMessage.Channel is IPrivateChannel || GamingUserList.Contains(rawMessage.Author.Id) || !message.HasStringPrefix(Setting.Prefix, ref argPos))
                return;

            SocketCommandContext context = new SocketCommandContext(Client, message);
            await Commands.ExecuteAsync(context, argPos, Services);
        }

        private Task OnClientLogReceived(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}