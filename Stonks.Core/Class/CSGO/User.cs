using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Core.Class.CSGO
{
    public class User
    {
        [JsonProperty("data")]
        public Data[] Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("platformId")]
        public long PlatformId { get; set; }

        [JsonProperty("platformSlug")]
        public string PlatformSlug { get; set; }

        [JsonProperty("platformUserIdentifier")]
        public string PlatformUserIdentifier { get; set; }

        [JsonProperty("platformUserId")]
        public string PlatformUserId { get; set; }

        [JsonProperty("platformUserHandle")]
        public string PlatformUserHandle { get; set; }

        [JsonProperty("avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("status")]
        public object Status { get; set; }

        [JsonProperty("additionalParameters")]
        public object AdditionalParameters { get; set; }
    }
}
