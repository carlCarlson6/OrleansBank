using Orleans.Runtime;
using Orleans.Storage;

namespace OrleansBank.Backennd.Infrastructure.Orleans;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder UseOrleans(this WebApplicationBuilder builder) 
    {
        builder.Host.UseOrleans(static silobuider =>
        {
            silobuider.UseLocalhostClustering();
        });

        builder.Services.AddSingletonNamedService<IGrainStorage, RavenDbGrainStateStorage>(nameof(RavenDbGrainStateStorage));

        return builder;
    }
}
