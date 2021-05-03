using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data
{
    public class WeirdGloopPrice : PriceEntity
    {
        /// <summary>
        /// The price of the item.
        /// </summary>
        [Required]
        [JsonPropertyName("price")]
        public long? Price { get; set; }

        /// <summary>
        /// The timestamp (UTC) the item was updated (in ISO8601 date format).
        /// </summary>
        [Required]
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
