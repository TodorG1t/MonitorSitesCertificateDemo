namespace MonitorSitesCertificate.Models
{
    public class MonitorSiteCertificatesResponse
    {
        public string? SiteUrl { get; set; }

        public CertificateStatusModel? CertificateStatus { get; set; }
    }
}
