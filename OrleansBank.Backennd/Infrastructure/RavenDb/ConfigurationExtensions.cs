using System.Security.Cryptography.X509Certificates;

namespace OrleansBank.Backennd.Infrastructure.RavenDb;

public static class ConfigurationExtensions
{
    public static (RavenSettings, X509Certificate2) GetRavenDbSettings(this ConfigurationManager configuration)
    {
        var dbSettings = configuration.GetSection(nameof(RavenSettings)).Get<RavenSettings>();

        X509Certificate2 certificate;
        if (!string.IsNullOrWhiteSpace(dbSettings!.Thumbprint))
        {
            certificate = LoadByThumbprint(dbSettings.Thumbprint);
        }
        else if (!string.IsNullOrWhiteSpace(dbSettings.CertContent))
        {
            var bytes = Convert.FromBase64String(dbSettings.CertContent);
            certificate = new X509Certificate2(bytes);
        }
        else
        {
            certificate = (!string.IsNullOrEmpty(dbSettings.CertPath)
                ? new X509Certificate2(dbSettings.CertPath, dbSettings.CertPass)
                : null)!;
        }

        return (dbSettings, certificate);
    }

    public static X509Certificate2 LoadByThumbprint(string thumbprint)
    {
        using var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        certStore.Open(OpenFlags.ReadOnly);

        var cert = certStore.Certificates.OfType<X509Certificate2>()
            .FirstOrDefault(x => x.Thumbprint == thumbprint);

        return cert;
    }
}
