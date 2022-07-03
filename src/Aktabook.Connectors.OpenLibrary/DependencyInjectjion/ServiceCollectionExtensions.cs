using Microsoft.Extensions.DependencyInjection;

namespace Aktabook.Connectors.OpenLibrary.DependencyInjectjion;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddOpenLibraryClient(
        this IServiceCollection services,
        OpenLibraryClientOptions options)
    {
        return services.AddHttpClient<IOpenLibraryClient, OpenLibraryClient>(
            client =>
            {
                client.BaseAddress = options.Host;
            });
    }
}