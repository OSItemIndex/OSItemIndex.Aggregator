using System;
using System.Text.Json.Serialization;
using OSItemIndex.Data.Extensions;

namespace OSItemIndex.Data
{
    public class RealtimeItemPrice : PriceEntity
    {
        public LatestPrice? Latest { get; set; }
        public AveragePrice? FiveMinute { get; set; }
        public AveragePrice? OneHour { get; set; }

        public class FiveMinutePrice : AveragePrice { }
        public class OneHourPrice : AveragePrice { }

        public class LatestPrice : PriceEntity
        {
            [JsonPropertyName("high")]
            public int? High { get; set; }

            [JsonPropertyName("highTime")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? HighTime { get; set; }

            [JsonPropertyName("low")]
            public int? Low { get; set; }

            [JsonPropertyName("lowTime")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? LowTime { get; set; }
        }

        public class AveragePrice : PriceEntity
        {
            [JsonPropertyName("avgHighPrice")]
            public int? AverageHighPrice { get; set; }

            [JsonPropertyName("highPriceVolume")]
            public int? HighPriceVolume { get; set; }

            [JsonPropertyName("avgLowPrice")]
            public int? AverageLowPrice { get; set; }

            [JsonPropertyName("lowPriceVolume")]
            public int? LowPriceVolume { get; set; }

            [JsonPropertyName("timestamp")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? Timestamp { get; set; }
        }
    }
}
