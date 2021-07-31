using Discord;

using System;
using System.Collections.Generic;

namespace Stonks.Core.Class
{
    public class ReactMessage
    {
        public ulong messageId { get; set; }
        public ulong messageGuildId { get; set; }
        public ulong messageChannelId { get; set; }
        public List<IEmote> messageEmote { get; set; }
        public List<Action> messageAction { get; set; }
        public ulong messageUserId { get; set; }
        public bool isNsfw { get; set; }
    }
}