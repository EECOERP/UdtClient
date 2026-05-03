using System.Globalization;

namespace UdtClient;

public sealed class ReflectionUdtMetadataMapper : IUdtMetadataMapper
{
    private readonly IUdtTypeMapRegistry _registry;

    public ReflectionUdtMetadataMapper(IUdtTypeMapRegistry registry)
    {
        _registry = registry;
    }

    public UdtInsertRequest MapInsert<T>(T dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return MapInsert(new[] { dto });
    }

    public UdtInsertRequest MapInsert<T>(IEnumerable<T> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);
        var items = dtos.ToList();
        if (items.Count == 0)
            throw new ArgumentException("At least one DTO is required.", nameof(dtos));

        var map = _registry.Get<T>();

        return new UdtInsertRequest
        {
            Table = map.TableName,
            Rows = items.Select(dto => new UdtInsertRow
            {
                Columns = BuildInsertColumns(dto!, map),
                Conditions = new List<object>()
            }).ToList()
        };
    }

    public UdtUpdateRequest MapUpdate<T>(T dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return MapUpdate(new[] { dto });
    }

    public UdtUpdateRequest MapUpdate<T>(IEnumerable<T> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);
        var items = dtos.ToList();
        if (items.Count == 0)
            throw new ArgumentException("At least one DTO is required.", nameof(dtos));

        var map = _registry.Get<T>();

        return new UdtUpdateRequest
        {
            Table = map.TableName,
            Rows = items.Select(dto => new UdtUpdateRow
            {
                Columns = BuildUpdateColumns(dto!, map),
                Conditions = new List<UdtRowUidCondition>
                {
                    new() { RowUid = GetRequiredUidValue(dto!, map) }
                }
            }).ToList()
        };
    }

    public UdtDeleteRequest MapDelete<T>(T dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return MapDelete(new[] { dto });
    }

    public UdtDeleteRequest MapDelete<T>(IEnumerable<T> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);
        var items = dtos.ToList();
        if (items.Count == 0)
            throw new ArgumentException("At least one DTO is required.", nameof(dtos));

        var map = _registry.Get<T>();

        return new UdtDeleteRequest
        {
            Table = map.TableName,
            Rows = items.Select(dto => new UdtDeleteRow
            {
                Conditions = new List<UdtRowUidCondition>
                {
                    new() { RowUid = GetRequiredUidValue(dto!, map) }
                }
            }).ToList()
        };
    }

    private static List<UdtColumn> BuildInsertColumns(object dto, UdtTypeMap map)
    {
        var columns = new List<UdtColumn>();

        if (map.UidAttribute.IncludeInInsertColumns)
        {
            columns.Add(new UdtColumn
            {
                Name = "rowuid",
                Value = GetRequiredUidValue(dto, map).ToString(CultureInfo.InvariantCulture)
            });
        }

        foreach (var column in map.Columns)
        {
            var rawValue = column.Property.GetValue(dto);
            if (rawValue is null && column.Attribute.IgnoreIfNull)
                continue;

            columns.Add(new UdtColumn
            {
                Name = column.Attribute.ColumnName,
                Value = ConvertToUdtString(rawValue)
            });
        }

        return columns;
    }

    private static List<UdtColumn> BuildUpdateColumns(object dto, UdtTypeMap map)
    {
        var columns = new List<UdtColumn>();

        foreach (var column in map.Columns)
        {
            var rawValue = column.Property.GetValue(dto);
            if (rawValue is null && column.Attribute.IgnoreIfNull)
                continue;

            columns.Add(new UdtColumn
            {
                Name = column.Attribute.ColumnName,
                Value = ConvertToUdtString(rawValue)
            });
        }

        return columns;
    }

    private static int GetRequiredUidValue(object dto, UdtTypeMap map)
    {
        var rawValue = map.UidProperty.GetValue(dto);
        if (rawValue is null)
        {
            throw new InvalidOperationException(
                $"UID property '{map.DtoType.FullName}.{map.UidProperty.Name}' cannot be null.");
        }

        var uid = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
        if (uid <= 0)
        {
            throw new InvalidOperationException(
                $"UID property '{map.DtoType.FullName}.{map.UidProperty.Name}' must be greater than zero.");
        }

        return uid;
    }

    private static string? ConvertToUdtString(object? value)
    {
        if (value is null) return null;

        return value switch
        {
            string s => s,
            DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTime dt => dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            Enum e => e.ToString(),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }
}
