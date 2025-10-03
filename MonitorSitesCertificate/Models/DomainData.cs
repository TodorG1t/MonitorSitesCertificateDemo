using System.Text.Json.Serialization;

namespace MonitorSitesCertificate.Models
{
    public class DomainData
    {
        [JsonPropertyName("contextName")]
        public string ContextName { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}