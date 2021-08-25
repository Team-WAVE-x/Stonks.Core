using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Stonks.Core.Rewrite.Class;
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
            _client.ReactionRemoved += OnReactionRemoved;
        }

        public void AddReactionMessage(RestUserMessage message, ulong userId, ulong guildId, Dictionary<IEmote, Action> dictionaries, TimeSpan timeout, bool removeMessageAfterTimeOut = false)
        {
            var cache = MemoryCache.Default;
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = timeout,
                RemovedCallback = CacheRemovedCallback
            };
            var reactMessage = new ReactMessage(message, userId, guildId, dictionaries, removeMessageAfterTimeOut);

            cache.Add(message.Id.ToString(), reactMessage, policy);

            foreach (var item in dictionaries)
            {
                message.AddReactionAsync(item.Key);
            }

            Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "React", $"Cached {message.Id}");
        }

        public void RemoveReactionMessage(ulong messageId)
        {
            var cache = MemoryCache.Default;
            cache.Remove(messageId.ToString());
        }

        private async void CacheRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            var reactMessage = arguments.CacheItem.Value as ReactMessage;

            try
            {
                if (reactMessage.RemoveMessageAfterTimeOut)
                {
                    var message = (RestUserMessage)await(_client.GetGuild(reactMessage.MessageGuildId).GetChannel(reactMessage.Message.Channel.Id) as ISocketMessageChannel).GetMessageAsync(reactMessage.Message.Id);
                    await message.DeleteAsync();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "React", $"Error deleting message! ({reactMessage.Message.Id})");
            }
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();

            if (reaction.UserId == _client.CurrentUser.Id)
            {
                await message.AddReactionAsync(reaction.Emote);
            }
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();
            var cache = MemoryCache.Default;

            if (message != null && reaction.User.IsSpecified && reaction.UserId != _client.CurrentUser.Id)
            {
                foreach (var items in cache)
                {
                    var reactMessage = items.Value as ReactMessage;

                    if (message.Id.ToString() == items.Key && reactMessage.Dictionaries.ContainsKey(reaction.Emote) && reactMessage.MessageUserId == reaction.UserId)
                    {
                        Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "React", $"{reaction.User.Value} Added React At {message.Id}");

                        foreach (var item in reactMessage.Dictionaries)
                        {
                            if (item.Key.Equals(reaction.Emote))
                            {
                                item.Value();
                            }
                        }

                        await message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    }
                }
            }
        }
    }
}