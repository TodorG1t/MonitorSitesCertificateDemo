using Microsoft.Extensions.Logging;
using MonitorSitesCertificate.Models;
using System.Security.Cryptography.X509Certificates;

namespace MonitorSitesCertificate.Utilities
{
    public static class SSLCertificateMonitor
    {
        public static async Task<List<MonitorSiteCertificatesResponse>> CheckCertificateStatusAsync(
            List<DomainData> listDomains,
            ILogger logger)
        {
            var listSiteMonitoringResponse = new List<MonitorSiteCertificatesResponse>();

            foreach (var domain in listDomains)
            {
                logger.LogInformation($"Verify SSL Certificate for {domain.ContextName} \n");

                try
                {
                    using var httpClient = new HttpClient();
                    var cert = await httpClient.GetServerCertificateAsync(domain.Url);

                    if (cert == null)
                    {
                        listSiteMonitoringResponse.Add(new MonitorSiteCertificatesResponse
                        {
                            SiteUrl = domain.Url,
                            CertificateStatus = new CertificateStatusModel
                            {
                                ContextName = domain.ContextName,
                                Url = domain.Url
                            }
                        });
                        
                        continue;
                    }

                    var daysUntilExpiry = (cert.NotAfter - DateTime.UtcNow).TotalDays;

                    listSiteMonitoringResponse.Add(new MonitorSiteCertificatesResponse
                    {
                        SiteUrl = domain.Url,
                        CertificateStatus = new CertificateStatusModel
                        {
                            ContextName = domain.ContextName,
                            Url = domain.Url,
                            Subject = cert.Subject,
                            DaysUntilExpiry = (int)daysUntilExpiry
                        }
                    });
                }
                catch (Exception ex)
                {
                    listSiteMonitoringResponse.Add(new MonitorSiteCertificatesResponse
                    {
                        SiteUrl = domain.Url,
                        CertificateStatus = new CertificateStatusModel
                        {
                            Url = domain.Url,
                            ErrorMessage = $"ERROR: {ex.Message}",
                        }
                    });
                }
            }

            return listSiteMonitoringResponse;
        }

        public static async Task<X509Certificate2?> GetServerCertificateAsync(this HttpClient httpClient, string url)
        {
            X509Certificate2? certificate = null;

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) =>
                {
                    if (cert != null)
                    {
                        certificate = new X509Certificate2(cert);
                    }
                    
                    return true;
                }
            };

            using var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(url);
                return certificate;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve certificate from {url}. Error: {ex.Message}");
            }
            finally
            {
                handler?.Dispose();
            }
        }
    }
}
