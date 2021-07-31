using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Stonks.Core.Class.CSGO
{
    public class Stat
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("platformInfo")]
        public PlatformInfo PlatformInfo { get; set; }

        [JsonProperty("userInfo")]
        public Dictionary<string, bool?> UserInfo { get; set; }

        [JsonProperty("segments")]
        public Segment[] Segments { get; set; }

        [JsonProperty("availableSegments")]
        public object[] AvailableSegments { get; set; }

        [JsonProperty("expiryDate")]
        public DateTimeOffset ExpiryDate { get; set; }
    }

    public class PlatformInfo
    {
        [JsonProperty("platformSlug")]
        public string PlatformSlug { get; set; }

        [JsonProperty("platformUserId")]
        public string PlatformUserId { get; set; }

        [JsonProperty("platformUserHandle")]
        public string PlatformUserHandle { get; set; }

        [JsonProperty("platformUserIdentifier")]
        public string PlatformUserIdentifier { get; set; }

        [JsonProperty("avatarUrl")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("additionalParameters")]
        public object AdditionalParameters { get; set; }
    }

    public class Segment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("expiryDate")]
        public DateTimeOffset ExpiryDate { get; set; }

        [JsonProperty("stats")]
        public Dictionary<string, StatValue> Stats { get; set; }
    }

    public class PurpleMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class StatValue
    {
        [JsonProperty("percentile")]
        public double? Percentile { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("displayCategory")]
        public DisplayCategory DisplayCategory { get; set; }

        [JsonProperty("category")]
        public Category Category { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("displayValue")]
        public string DisplayValue { get; set; }

        [JsonProperty("displayType")]
        public DisplayType DisplayType { get; set; }
    }

    public enum Category { Combat, General, Objective, Round };

    public enum DisplayCategory { Combat, General, Objective, Round };

    public enum DisplayType { Number, NumberPercentage, NumberPrecision2, TimeSeconds };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CategoryConverter.Singleton,
                DisplayCategoryConverter.Singleton,
                DisplayTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Category) || t == typeof(Category?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<string>(reader);

            switch (value)
            {
                case "combat":
                    return Category.Combat;
                case "general":
                    return Category.General;
                case "objective":
                    return Category.Objective;
                case "round":
                    return Category.Round;
                default:
                    throw new InvalidOperationException("Unknown Type");
            }

            throw new InvalidCastException("Cannot unmarshal type Category");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Category)untypedValue;
            switch (value)
            {
                case Category.Combat:
                    serializer.Serialize(writer, "combat");
                    return;
                case Category.General:
                    serializer.Serialize(writer, "general");
                    return;
                case Category.Objective:
                    serializer.Serialize(writer, "objective");
                    return;
                case Category.Round:
                    serializer.Serialize(writer, "round");
                    return;
                default:
                    throw new InvalidOperationException("Unknown Type");
            }
            throw new InvalidCastException("Cannot marshal type Category");
        }

        public static readonly CategoryConverter Singleton = new CategoryConverter();
    }

    internal class DisplayCategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DisplayCategory) || t == typeof(DisplayCategory?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<string>(reader);

            switch (value)
            {
                case "Combat":
                    return DisplayCategory.Combat;
                case "General":
                    return DisplayCategory.General;
                case "Objective":
                    return DisplayCategory.Objective;
                case "Round":
                    return DisplayCategory.Round;
                default:
                    throw new InvalidOperationException("Unknown Type");
            }

            throw new InvalidCastException("Cannot unmarshal type DisplayCategory");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (DisplayCategory)untypedValue;

            switch (value)
            {
                case DisplayCategory.Combat:
                    serializer.Serialize(writer, "Combat");
                    return;
                case DisplayCategory.General:
                    serializer.Serialize(writer, "General");
                    return;
                case DisplayCategory.Objective:
                    serializer.Serialize(writer, "Objective");
                    return;
                case DisplayCategory.Round:
                    serializer.Serialize(writer, "Round");
                    return;
                default:
                    throw new InvalidOperationException("Unknown Type");
            }

            throw new InvalidCastException("Cannot marshal type DisplayCategory");
        }

        public static readonly DisplayCategoryConverter Singleton = new DisplayCategoryConverter();
    }

    internal class DisplayTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DisplayType) || t == typeof(DisplayType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<string>(reader);

            switch (value)
            {
                case "Number":
                    return DisplayType.Number;
                case "NumberPercentage":
                    return DisplayType.NumberPercentage;
                case "NumberPrecision2":
                    return DisplayType.NumberPrecision2;
                case "TimeSeconds":
                    return DisplayType.TimeSeconds;
                default:
                    throw new InvalidOperationException("Unknown Type");
            }

            throw new InvalidCastException("Cannot unmarshal type DisplayType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (DisplayType)untypedValue;

            switch (value)
            {
                case DisplayType.Number:
                    serializer.Serialize(writer, "Number");
                    return;
                case DisplayType.NumberPercentage:
                    serializer.Serialize(writer, "NumberPercentage");
                    return;
                case DisplayType.NumberPrecision2:
                    serializer.Serialize(writer, "NumberPrecision2");
                    return;
                case DisplayType.TimeSeconds:
                    serializer.Serialize(writer, "TimeSeconds");
                    return;
                default:
                    throw new InvalidOperationException("Unknown Type");
            }

            throw new InvalidCastException("Cannot marshal type DisplayType");
        }

        public static readonly DisplayTypeConverter Singleton = new DisplayTypeConverter();
    }
}
