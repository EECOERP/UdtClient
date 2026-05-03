using System.Reflection;

namespace UdtClient;

internal sealed class UdtRegistrationBuilder : IUdtRegistrationBuilder
{
    private readonly List<Type> _dtoTypes = new();

    public IReadOnlyList<Type> DtoTypes => _dtoTypes;

    public IUdtRegistrationBuilder AddDto<T>() where T : class
    {
        var type = typeof(T);

        if (type.GetCustomAttribute<UdtTableAttribute>() is null)
            throw new InvalidOperationException(
                $"'{type.Name}' is missing [UdtTable].");

        if (!type.GetProperties().Any(p => p.GetCustomAttribute<UdtUidAttribute>() is not null))
            throw new InvalidOperationException(
                $"'{type.Name}' has no [UdtUid] property.");

        if (!type.GetProperties().Any(p => p.GetCustomAttribute<UdtColumnAttribute>() is not null))
            throw new InvalidOperationException(
                $"'{type.Name}' has no [UdtColumn] properties.");

        _dtoTypes.Add(type);
        return this;
    }
}
