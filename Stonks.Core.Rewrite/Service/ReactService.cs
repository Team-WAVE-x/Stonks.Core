using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Stonks.Core.Rewrite.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Service
{
    public class ReactService
    {
        private IServiceProvider _service;
        private DiscordSocketClient _client;

        public ReactService(IServiceProvider service)
        {
            _service = service;
            _client = service.GetRequiredService<DiscordSocketClient>();
            _client.ReactionAdded += OnReactionAdded;
        }

        public static void AddReactionMessage(RestUserMessage message, ulong userId, List<Dictionary<IEmote, Action>> dictionaries, TimeSpan timeout, bool removeMessageAfterTimeOut = false)
        {
            ObjectCache cache = MemoryCache.Default;
        }

        public static void RemoveReactionMessage(ulong messageId)
        {
            ObjectCache cache = MemoryCache.Default;
        }

        private void CacheRemovedCallback(CacheEntryRemovedArguments arguments)
        {

        }

        private Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }
    }
}