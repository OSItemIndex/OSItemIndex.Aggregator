using System;

namespace OSItemIndex.Data
{
    public class EntityRepoVersion
    {
        public string Version { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public object? Data { get; set; }
    }
}
