using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;

namespace OrleansBank.Backennd.Infrastructure.RavenDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRavenDb(this IServiceCollection services, ConfigurationManager config)
    {
        var (dbSettings, cert) = config.GetRavenDbSettings();

        var store = new DocumentStore
        {
            Urls = dbSettings.Urls,
            Database = dbSettings.DatabaseName,
            Certificate = cert,

        }.Initialize();

        return services.AddSingleton(_ => store);
    }
}

public class RavenSettings
{
    public string[] Urls { get; set; } = [];
    public string DatabaseName { get; set; } = string.Empty;
    public string? CertPath { get; set; }
    public string? CertPass { get; set; }
    public string? Thumbprint { get; set; }
    public string? SecondaryThumbprint { get; set; }
    public string? CertContent { get; set; }
    public string[] Profile { get; set; } = [];

    public static (RavenSettings, X509Certificate2) FromConfig(IConfiguration configuration, string sectionName = null!)
    {
        var dbSettings = new RavenSettings();
        configuration.Bind(sectionName ?? nameof(RavenSettings), dbSettings);
        var certificate = !string.IsNullOrEmpty(dbSettings?.CertPath)
            ? new X509Certificate2(dbSettings.CertPath, dbSettings.CertPass)
            : null;
        return (dbSettings, certificate);
    }
}