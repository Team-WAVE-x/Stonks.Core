using Discord;
using Discord.Rest;
using Discord.WebSocket;

using Stonks.Core.Class;

using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Stonks.Core.Module
{
    public static class ReactMessageModule
    {
        /// <summary>
        /// React에 반응하는 메시지 등록
        /// </summary>
        /// <param name="msg">메시지 객체</param>
        /// <param name="emoji">이모지</param>
        /// <param name="action">대리자</param>
        /// <param name="timeSpan">캐시를 보관할 기간</param>
        /// <param name="userId">유저 아이디</param>
        public static void CreateReactMessage(RestUserMessage msg, List<IEmote> emoji, List<Action> action, TimeSpan timeSpan, ulong userId, ulong guildId, bool nsfw = false)
        {
            if (emoji.Count != action.Count)
            {
                throw new ArgumentException("Emoji list and Action list must have the same length.");
            }

            MemoryCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy 
            {
                SlidingExpiration = timeSpan,
                RemovedCallback = new CacheEntryRemovedCallback(CacheRemovedCallback),
            };

            foreach (IEmote item in emoji)
            {
                msg.AddReactionAsync(item);
            }

            cache.Add(msg.Id.ToString(), new ReactMessage 
            { 
                messageId = msg.Id, 
                messageChannelId = msg.Channel.Id, 
                messageGuildId = guildId, 
                messageEmote = emoji, 
                messageAction = action, 
                messageUserId = userId, 
                isNsfw = nsfw 
            }, policy);

            Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "React", $"Cached {msg.Id}");
        }

        /// <summary>
        /// 캐시가 메모리에서 삭제된다면 실행되는 메서드
        /// </summary>
        /// <param name="arguments">매개 변수</param>
        private async static void CacheRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            var reactMessage = arguments.CacheItem.Value as ReactMessage;

            try
            {
                if (reactMessage.isNsfw)
                {
                    var message = (RestUserMessage)await (Program.Client.GetGuild(reactMessage.messageGuildId).GetChannel(reactMessage.messageChannelId) as ISocketMessageChannel).GetMessageAsync(reactMessage.messageId);

                    await message.DeleteAsync();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("{0} {1,-11} {2}", DateTime.Now.ToString("HH:mm:ss"), "React", $"Error deleting message! ({reactMessage.messageId})");
            }
        }

        /// <summary>
        /// 반응 메시지 삭제
        /// </summary>
        /// <param name="messageId">메시지 아이디</param>
        public static void RemoveReactMessage(ulong messageId)
        {
            MemoryCache cache = MemoryCache.Default;
            cache.Remove(messageId.ToString());
        }
    }
}