using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Class
{
    public class Setting
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("trn_token")]
        public string TRNToken { get; set; }

        [JsonProperty("koreanbots_token")]
        public string KoreanbotsToken { get; set; }

        [JsonProperty("connection_string")]
        public string ConnectionString { get; set; }

        [JsonProperty("developer_id")]
        public ulong DeveloperID { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("error_log_channel_id")]
        public ulong ErrorLogChannelID { get; set; }

        [JsonProperty("koreanbots_id")]
        public ulong KoreanbotsID { get; set; }
    }
}
