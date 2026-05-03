using Microsoft.Extensions.Hosting;

namespace UdtClient;

internal sealed class UdtRegistrationStartupService : IHostedService
{
    private readonly UdtRegistrationBuilder _builder;
    private readonly IUdtTypeMapRegistry _registry;

    public UdtRegistrationStartupService(UdtRegistrationBuilder builder, IUdtTypeMapRegistry registry)
    {
        _builder = builder;
        _registry = registry;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_builder.DtoTypes.Count == 0)
        {
            throw new InvalidOperationException(
                "No UDT DTO types were registered. Register at least one DTO in Program.cs.");
        }

        foreach (var dtoType in _builder.DtoTypes.Distinct())
        {
            _registry.Register(dtoType);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
