using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using Stonks.Core.Rewrite.Class;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Service
{
    public class DiscordService
    {
        private Setting _setting;
        private ServiceProvider _service;
        private DiscordSocketClient _client;

        public DiscordService()
        {
            _service = ConfigureServices();
            _setting = _service.GetRequiredService<Setting>();
            _client = _service.GetRequiredService<DiscordSocketClient>();
        }

        public async Task MainAsync()
        {
            _setting.GetConfig($"{AppDomain.CurrentDomain.BaseDirectory}\\Setting.json");
            _client.Log += new LoggingService().OnClientLogReceived;
            _service.GetRequiredService<CommandService>().Log += new LoggingService().OnClientLogReceived;

            await _client.LoginAsync(TokenType.Bot, _setting.Config.Token);
            await _client.StartAsync();
            await _client.SetGameAsync($"{_setting.Config.Prefix}도움말", null, ActivityType.Listening);
            await _service.GetRequiredService<CommandHandlingService>().InitializeAsync();
            await Task.Delay(Timeout.Infinite).ConfigureAwait(false);
        }

        public ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<Setting>()
                .BuildServiceProvider();
        }
    }
}