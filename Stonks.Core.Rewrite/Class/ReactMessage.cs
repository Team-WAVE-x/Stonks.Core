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
        public List<Dictionary<IEmote, Action>> Dictionaries { get; }
        public bool RemoveMessageAfterTimeOut { get; }

        public ReactMessage(RestUserMessage message, ulong messageUserId, List<Dictionary<IEmote, Action>> dictionaries, bool removeMessageAfterTimeOut)
        {
            this.Message = message;
            this.MessageUserId = messageUserId;
            this.Dictionaries = dictionaries;
            this.RemoveMessageAfterTimeOut = removeMessageAfterTimeOut;
        }
    }
}