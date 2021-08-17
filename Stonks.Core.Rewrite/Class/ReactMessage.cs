using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Rewrite.Class
{
    public class ReactMessage
    {
        public RestUserMessage Message { get; }
        public ulong MessageUserId { get; }
        public ulong MessageGuildId { get; }
        public Dictionary<IEmote, Action> Dictionaries { get; }
        public bool RemoveMessageAfterTimeOut { get; }

        public ReactMessage(RestUserMessage message, ulong messageUserId, ulong messageGuildId ,Dictionary<IEmote, Action> dictionaries, bool removeMessageAfterTimeOut)
        {
            this.Message = message;
            this.MessageUserId = messageUserId;
            this.MessageGuildId = messageGuildId;
            this.Dictionaries = dictionaries;
            this.RemoveMessageAfterTimeOut = removeMessageAfterTimeOut;
        }
    }
}