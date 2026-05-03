using System.Reflection;

namespace UdtClient;

public sealed class ReflectionUdtTypeMapFactory : IUdtTypeMapFactory
{
    public UdtTypeMap Create(Type dtoType)
    {
        ArgumentNullException.ThrowIfNull(dtoType);

        var tableAttribute = dtoType.GetCustomAttribute<UdtTableAttribute>();
        if (tableAttribute is null)
        {
            throw new InvalidOperationException(
                $"Type '{dtoType.FullName}' must be decorated with [{nameof(UdtTableAttribute)}].");
        }

        var properties = dtoType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead)
            .ToList();

        var uidProperties = properties
            .Where(p => p.GetCustomAttribute<UdtUidAttribute>() is not null)
            .ToList();

        if (uidProperties.Count != 1)
        {
            throw new InvalidOperationException(
                $"Type '{dtoType.FullName}' must define exactly one property with [{nameof(UdtUidAttribute)}].");
        }

        var uidProperty = uidProperties[0];
        var uidAttribute = uidProperty.GetCustomAttribute<UdtUidAttribute>()!;

        if (!IsSupportedUidType(uidProperty.PropertyType))
        {
            throw new InvalidOperationException(
                $"Property '{dtoType.FullName}.{uidProperty.Name}' marked with [{nameof(UdtUidAttribute)}] must be an integer type.");
        }

        var mappedColumns = properties
            .Select(p => new
            {
                Property = p,
                ColumnAttribute = p.GetCustomAttribute<UdtColumnAttribute>()
            })
            .Where(x => x.ColumnAttribute is not null)
            .Select(x => new UdtMappedColumn(x.Property, x.ColumnAttribute!))
            .ToList();

        return new UdtTypeMap(
            dtoType,
            tableAttribute.TableName,
            uidProperty,
            uidAttribute,
            mappedColumns);
    }

    private static bool IsSupportedUidType(Type type)
    {
        var actualType = Nullable.GetUnderlyingType(type) ?? type;

        return actualType == typeof(byte)
            || actualType == typeof(short)
            || actualType == typeof(int)
            || actualType == typeof(long);
    }
}
