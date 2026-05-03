using System.Collections.Concurrent;

namespace UdtClient;

public sealed class UdtTypeMapRegistry : IUdtTypeMapRegistry
{
    private readonly IUdtTypeMapFactory _factory;
    private readonly ConcurrentDictionary<Type, UdtTypeMap> _maps = new();

    public UdtTypeMapRegistry(IUdtTypeMapFactory factory)
    {
        _factory = factory;
    }

    public void Register(Type dtoType)
    {
        ArgumentNullException.ThrowIfNull(dtoType);

        if (_maps.ContainsKey(dtoType))
        {
            throw new InvalidOperationException(
                $"DTO type '{dtoType.FullName}' has already been registered.");
        }

        var map = _factory.Create(dtoType);
        if (!_maps.TryAdd(dtoType, map))
        {
            throw new InvalidOperationException(
                $"DTO type '{dtoType.FullName}' could not be registered.");
        }
    }

    public void Register<T>() => Register(typeof(T));

    public UdtTypeMap Get(Type dtoType)
    {
        ArgumentNullException.ThrowIfNull(dtoType);

        if (_maps.TryGetValue(dtoType, out var map))
        {
            return map;
        }

        throw new InvalidOperationException(
            $"DTO type '{dtoType.FullName}' has not been registered. Register it in Program.cs with AddDto<{dtoType.Name}>().");
    }

    public UdtTypeMap Get<T>() => Get(typeof(T));
}
