using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UdtClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUdtClient(
        this IServiceCollection services,
        Action<UdtOptions> configureOptions,
        Action<IUdtRegistrationBuilder> configureDtos)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        ArgumentNullException.ThrowIfNull(configureDtos);

        services.Configure(configureOptions);
        services.AddSingleton<IUdtTypeMapFactory, ReflectionUdtTypeMapFactory>();
        services.AddSingleton<IUdtTypeMapRegistry, UdtTypeMapRegistry>();
        services.AddSingleton<IUdtMetadataMapper, ReflectionUdtMetadataMapper>();

        var builder = new UdtRegistrationBuilder();
        configureDtos(builder);
        services.AddSingleton(builder);
        services.AddHostedService<UdtRegistrationStartupService>();

        services.AddHttpClient<IUdtClient, UdtClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<UdtOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }
        });

        return services;
    }
}
