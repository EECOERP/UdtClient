using System.Reflection;

namespace UdtClient;

public sealed class ReflectionUdtTypeMapFactory : IUdtTypeMapFactory
{
    public UdtTypeMap Create(Type dtoType)
    {
        ArgumentNullException.ThrowIfNull(dtoType);

        var tableAttr = dtoType.GetCustomAttribute<UdtTableAttribute>()
            ?? throw new InvalidOperationException(
                $"'{dtoType.Name}' is missing [UdtTable]. All IUdtDto types must declare a table name.");

        var properties = dtoType.GetProperties();

        PropertyInfo? uidProp = null;
        UdtUidAttribute? uidAttr = null;
        var columns = new List<UdtMappedColumn>();

        foreach (var prop in properties)
        {
            var uid = prop.GetCustomAttribute<UdtUidAttribute>();
            if (uid is not null)
            {
                if (uidProp is not null)
                    throw new InvalidOperationException(
                        $"'{dtoType.Name}' has more than one [UdtUid] property.");

                uidProp = prop;
                uidAttr = uid;
            }
            else if (prop.GetCustomAttribute<UdtColumnAttribute>() is { } colAttr)
            {
                columns.Add(new UdtMappedColumn(prop, colAttr));
            }
        }

        if (uidProp is null || uidAttr is null)
            throw new InvalidOperationException(
                $"'{dtoType.Name}' has no [UdtUid] property. Exactly one property must carry [UdtUid].");

        return new UdtTypeMap(dtoType, tableAttr.TableName, uidProp, uidAttr, columns);
    }
}
