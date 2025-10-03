# SSL Certificate Monitoring Azure Function

An Azure Function that monitors SSL certificates for specified domains and tracks their expiration status using Azure Application Insights.

## Overview

The [`SSLCertificateMonitoringFunction`](MonitorSitesCertificate/SSLCertificateMonitoringFunction.cs) is a timer-triggered Azure Function that:
- Reads domain configurations from a JSON file
- Checks SSL certificates for each domain
- Tracks certificate metrics in Application Insights

## Configuration

### Timer Schedule

The function runs on a schedule defined by a cron expression:
```csharp
[TimerTrigger("*/3 * * * * *")]
```
Currently set to run every 3 seconds (for testing). In production, you should adjust this to a more appropriate interval.

### Domain Configuration

Domains to monitor are configured in [domains.json](MonitorSitesCertificate/domains.json):
```json
[
  {
    "contextName": "Dev BG",
    "url": "https://dev.bg"
  }
]
```

### Application Settings

Required environment variables:
- `TelemetryConfigurationString`: Connection string for Azure Application Insights

## Components

### Main Components

1. [`SSLCertificateMonitoringFunction`](MonitorSitesCertificate/SSLCertificateMonitoringFunction.cs)
   - Entry point for the timer-triggered function
   - Orchestrates the certificate monitoring process

2. [`SSLCertificateMonitor`](MonitorSitesCertificate/Utilities/SSLCertificateMonitor.cs)
   - Handles the certificate validation logic
   - Retrieves SSL certificates from domains
   - Calculates days until expiration

3. [`AppInsightTelemetryClient`](MonitorSitesCertificate/Clients/AppInsightTelemetryClient.cs)
   - Manages telemetry reporting to Azure Application Insights
   - Tracks certificate metrics and events

### Data Models

- [`DomainData`](MonitorSitesCertificate/Models/DomainData.cs): Configuration model for domains to monitor
- [`CertificateStatusModel`](MonitorSitesCertificate/Models/CertificateStatusModel.cs): Certificate information model
- [`MonitorSiteCertificatesResponse`](MonitorSitesCertificate/Models/MonitorSiteCertificatesResponse.cs): Response model for certificate checks

## Metrics Tracked

The following metrics are tracked in Application Insights:
- Days until certificate expiration
- Certificate check events with properties:
  - Context
  - URL
  - Subject
  - Days until expiry

## Dependencies

- .NET 8.0
- Azure Functions v4
- Azure Application Insights
- Microsoft.Azure.Functions.Worker
- Newtonsoft.Json

## Development

### Local Development

1. Create a `local.settings.json` file with required configuration:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "TelemetryConfigurationString": "your-appinsights-connection-string"
  }
}
```

2. Build and run:
```sh
dotnet build
func start
```

### Deployment

Deploy to Azure using Visual Studio or Azure CLI. Ensure you configure the following:
- Application settings including the Application Insights connection string
- Appropriate hosting plan
- Correct timer schedule for your needs