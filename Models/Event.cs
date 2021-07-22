using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum EventType
    {
        [EnumMember(Value = "seed")] Seed,
        [EnumMember(Value = "update")] Update
    }

    [Flags]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum EventSource
    {
        [EnumMember(Value = "items")] Items = 1 << 0,
        [EnumMember(Value = "prices")] Prices = 1 << 1,
        [EnumMember(Value = "realtime_latest")] PricesRealtimeLatest = 1 << 2,
        [EnumMember(Value = "realtime_5m")] PricesRealtimeFiveMinute = 1 << 3,
        [EnumMember(Value = "realtime_1h")] PricesRealtimeOneHour = 1 << 4,
    }

    public class Event
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public EventType Type { get; set; }
        public EventSource Source { get; set; }

        [Column(TypeName = "jsonb")]
        public object? Details { get; set; }
    }
}
