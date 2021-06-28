using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data
{
    public class OsrsBoxItem : ItemEntity
    {
        /// <summary>
        /// If the item is a duplicate.
        /// </summary>
        [JsonPropertyName("duplicate")]
        public bool? Duplicate { get; set; }

        /// <summary>
        /// If the item is noted.
        /// </summary>
        [JsonPropertyName("noted")]
        public bool? Noted { get; set; }

        /// <summary>
        /// If the item is a placeholder.
        /// </summary>
        [JsonPropertyName("placeholder")]
        public bool? Placeholder { get; set; }

        /// <summary>
        /// If the item is stackable (in inventory).
        /// </summary>
        [JsonPropertyName("stackable")]
        public bool? Stackable { get; set; }

        /// <summary>
        /// If the item is tradeable (only on GE).
        /// </summary>
        [JsonPropertyName("tradeable_on_ge")]
        public bool? TradeableOnGe { get; set; }

        /// <summary>
        /// The last time (UTC) the item was updated (in ISO8601 date format).
        /// </summary>
        [JsonPropertyName("last_updated")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// OSRSBox item document.
        /// </summary>
        [Column(TypeName = "json")]
        [JsonPropertyName("document")]
        public string? Document { get; set; }
    }
}
