namespace MonitorSitesCertificate.Models
{
    public class CertificateStatusModel
    {
        public string? ContextName { get; set; }

        public string? Url { get; set; }

        public string? Subject { get; set; }

        public string? Issuer { get; set; }

        public string? SerialNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? DaysUntilExpiry { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
