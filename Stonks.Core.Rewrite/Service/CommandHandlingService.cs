﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using Stonks.Core.Rewrite.Class;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Service
{
    public class CommandHandlingService
    {
        private Setting _setting;
        private CommandService _command;
        private IServiceProvider _service;
        private DiscordSocketClient _client;

        public CommandHandlingService(IServiceProvider service)
        {
            _service = service;
            _setting = service.GetRequiredService<Setting>();
            _command = service.GetRequiredService<CommandService>();
            _client = service.GetRequiredService<DiscordSocketClient>();

            _client.MessageReceived += OnClientMessage;
            _command.Log += new LoggingService().OnCommandLogReceived;
            _command.CommandExecuted += OnCommandExecuted;
        }

        public async Task InitializeAsync()
        {
            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _service);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess)
                return;

            var builder = new EmbedBuilder
            {
                Title = "❌ 오류",
                Color = Color.Red,
                Description = $"`{result}`",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = _client.CurrentUser.GetAvatarUrl(),
                    Text = _client.CurrentUser.Username
                },
                Timestamp = DateTimeOffset.Now
            };

            await (context.Client.GetChannelAsync(_setting.Config.ErrorLogChannelId).Result as ISocketMessageChannel).SendMessageAsync(embed: builder.Build());
        }

        private async Task OnClientMessage(SocketMessage socketMessage)
        {
            int argPos = 0;

            if (!(socketMessage is SocketUserMessage message) || message.Source != MessageSource.User || socketMessage.Channel is IPrivateChannel || !message.HasStringPrefix(_setting.Config.Prefix, ref argPos))
                return;

            SocketCommandContext context = new SocketCommandContext(_client, message);
            await _command.ExecuteAsync(context, argPos, _service);
        }
    }
}