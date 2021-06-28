using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data
{
    public class EntityRepoUpdateRequest<T> where T : class
    {
        [JsonPropertyName("version")]
        public EntityRepoVersion? Version { get; set; }

        [JsonPropertyName("entities")]
        public IEnumerable<T> Entities { get; set; }
    }
}
