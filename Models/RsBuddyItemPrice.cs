using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data
{
    public class RsBuddyItemPrice : PriceEntity
    {
        /// <summary>
        /// Store price.
        /// </summary>
        [Required]
        [JsonPropertyName("sp")]
        public int? Sp { get; set; }

        /// <summary>
        /// Average buy price.
        /// </summary>
        [Required]
        [JsonPropertyName("buy_average")]
        public int? BuyAverage { get; set; }

        /// <summary>
        /// Average buy quantity.
        /// </summary>
        [Required]
        [JsonPropertyName("buy_quantity")]
        public int? BuyQuantity { get; set; }

        /// <summary>
        /// Average sell price.
        /// </summary>
        [Required]
        [JsonPropertyName("sell_average")]
        public int? SellAverage { get; set; }

        /// <summary>
        /// Average sell quantity.
        /// </summary>
        [Required]
        [JsonPropertyName("sell_quantity")]
        public int? SellQuantity { get; set; }

        /// <summary>
        /// Average overall price.
        /// </summary>
        [Required]
        [JsonPropertyName("overall_average")]
        public int? OverallAverage { get; set; }

        /// <summary>
        /// Average overall quantity.
        /// </summary>
        [Required]
        [JsonPropertyName("sell_quantity")]
        public int? OverallQuantity { get; set; }

        /// <summary>
        /// The timestamp (UTC) the item was updated (in ISO8601 date format).
        /// </summary>
        [Required]
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
