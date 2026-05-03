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

        var options = new UdtOptions();
        configureOptions(options);

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new ArgumentException("UdtOptions.BaseUrl is required.", nameof(configureOptions));

        if (string.IsNullOrWhiteSpace(options.BearerToken))
            throw new ArgumentException("UdtOptions.BearerToken is required.", nameof(configureOptions));

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
            var opts = sp.GetRequiredService<IOptions<UdtOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
        });

        return services;
    }
}
