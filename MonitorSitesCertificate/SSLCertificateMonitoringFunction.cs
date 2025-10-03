using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MonitorSitesCertificate.Clients;
using MonitorSitesCertificate.Models;
using MonitorSitesCertificate.Utilities;
using Newtonsoft.Json;

namespace MonitorSitesCertificate;

public class SSLCertificateMonitoringFunction
{
    private readonly ILogger _logger;
    private AppInsightTelemetryClient _telemetryClient;

    public SSLCertificateMonitoringFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SSLCertificateMonitoringFunction>();
        _telemetryClient = new AppInsightTelemetryClient();
    }

    [Function("SSLCertificateMonitoringFunction")]
    public async Task Run([TimerTrigger("*/3 * * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("Start SSL Certificate Monitoring function at: {executionTime}\n", DateTime.UtcNow);

        string path = Path.Combine(AppContext.BaseDirectory, "domains.json");
        string jsonFileContent = await File.ReadAllTextAsync(path);
        var listTenants = JsonConvert.DeserializeObject<List<DomainData>>(jsonFileContent);

        var listSites = await SSLCertificateMonitor.CheckCertificateStatusAsync(
            listDomains: listTenants,
            logger: _logger);

        foreach (var site in listSites)
        {
            _telemetryClient.TrackCertificateMetrics(site.CertificateStatus, log: _logger);
        }

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}