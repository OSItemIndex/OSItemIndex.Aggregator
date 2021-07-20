using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace OSItemIndex.Data
{
    public enum EventType
    {
        [EnumMember(Value = "seed")] Seed,
        [EnumMember(Value = "update")] Update
    }

    [Flags]
    public enum EventSource
    {
        [EnumMember(Value = "items")] Items = 1 << 0,
        [EnumMember(Value = "prices")] Prices = 1 << 1,
        [EnumMember(Value = "prices_realtime_latest")] PricesRealtimeLatest = 1 << 2,
        [EnumMember(Value = "prices_realtime_5m")] PricesRealtimeFiveMinute = 1 << 3,
        [EnumMember(Value = "prices_realtime_1h")] PricesRealtimeOneHour = 1 << 4,
    }

    public class Event
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public EventType Type { get; set; }
        public EventSource Source { get; set; }

        [Column(TypeName = "json")]
        public object? Details { get; set; } // manually passed
    }
}
