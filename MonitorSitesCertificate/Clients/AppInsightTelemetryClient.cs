using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using MonitorSitesCertificate.Models;

namespace MonitorSitesCertificate.Clients
{
    public class AppInsightTelemetryClient
    {
        private TelemetryConfiguration telemetryConfiguration;
        private TelemetryClient telemetryClient;

        public AppInsightTelemetryClient()
        {
            this.telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            this.telemetryConfiguration.ConnectionString = Environment.GetEnvironmentVariable("TelemetryConfigurationString");
            this.telemetryClient = new TelemetryClient(this.telemetryConfiguration);
        }

        public void TrackCertificateMetrics(CertificateStatusModel certificate, ILogger log)
        {
            var properties = new Dictionary<string, string>
            {
                { "Context", certificate.ContextName ?? "Unknown context" },
                { "URL", certificate.Url ?? "Unknown URL" },
                { "Subject", certificate.Subject ?? "Unknown subject" },
                { "DaysUntilExpiry", certificate?.DaysUntilExpiry?.ToString() ?? "Unknown days until expiry" }
            };

            var metrics = new Dictionary<string, double>();

            if (certificate.DaysUntilExpiry.HasValue)
            {
                metrics.Add("DaysUntilExpiry", certificate.DaysUntilExpiry.Value);
            }

            // Track an event for certificate check
            log.LogInformation("CertificateCheck Properties: {@Properties}", properties);
            this.telemetryClient.TrackEvent("CertificateStatusChecked", properties, metrics);
            this.telemetryClient.Flush();
        }

        public TelemetryClient AppInsightTelemetry => this.telemetryClient;
    }
}
