using System;
using System.Text.Json.Serialization;

namespace OSItemIndex.Updater
{
    /// <summary>
    /// https://release-monitoring.org/api/{projectid} response model.
    /// </summary>
    public class ReleaseMonitoringProject
    {
        [JsonPropertyName("backend")]
        public string Backend { get; set; }

        [JsonPropertyName("created_on")]
        public double CreatedOn { get; set; }

        [JsonPropertyName("ecosystem")]
        public string Ecosystem { get; set; }

        [JsonPropertyName("homepage")]
        public Uri Homepage { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("packages")]
        public object[] Packages { get; set; }

        [JsonPropertyName("regex")]
        public object Regex { get; set; }

        [JsonPropertyName("stable_versions")]
        public string[] StableVersions { get; set; }

        [JsonPropertyName("updated_on")]
        public double UpdatedOn { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("version_url")]
        public object VersionUrl { get; set; }

        [JsonPropertyName("versions")]
        public string[] Versions { get; set; }
    }
}
