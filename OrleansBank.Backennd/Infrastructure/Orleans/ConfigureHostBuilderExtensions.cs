namespace OrleansBank.Backennd.Infrastructure.Orleans;

public static class ConfigureHostBuilderExtensions
{
    public static void AddOrleans(this ConfigureHostBuilder configureHostBuilder) =>
        configureHostBuilder.UseOrleans(static silobuider =>
        {
            silobuider.UseLocalhostClustering();
        });
}
