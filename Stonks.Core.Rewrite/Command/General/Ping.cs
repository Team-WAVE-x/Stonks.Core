using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Command.General
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("핑", RunMode = RunMode.Async)]
        public async Task PingAsync()
        {
            var message = await Context.Channel.SendMessageAsync($"Pinging...");
            var latency = message.Timestamp - Context.Message.Timestamp;
            var pingColor = new Color();

            if (latency.TotalMilliseconds < 50)
                pingColor = Color.Green;
            else if (latency.TotalMilliseconds < 150)
                pingColor = Color.LightOrange;
            else if (latency.TotalMilliseconds < 300)
                pingColor = Color.Orange;
            else
                pingColor = Color.Red;

            var builder = new EmbedBuilder()
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
    }
}
