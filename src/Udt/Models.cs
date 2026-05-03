using System.Reflection;

namespace UdtClient;

public sealed class UdtOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string InsertPath { get; set; } = "/udtservice/api/udtdata/insertudtdata";
    public string UpdatePath { get; set; } = "/udtservice/api/udtdata/updateudtdata";
    public string DeletePath { get; set; } = "/udtservice/api/udtdata/deleteudtdata";
    public string? BearerToken { get; set; }
    public string? ApiKey { get; set; }
}

public sealed class UdtColumn
{
    public string Name { get; init; } = string.Empty;
    public string? Value { get; init; }
}

public sealed class UdtRowUidCondition
{
    public int RowUid { get; init; }
}

public sealed class UdtInsertRow
{
    public List<UdtColumn> Columns { get; init; } = new();
    public List<object> Conditions { get; init; } = new();
}

public sealed class UdtUpdateRow
{
    public List<UdtColumn> Columns { get; init; } = new();
    public List<UdtRowUidCondition> Conditions { get; init; } = new();
}

public sealed class UdtDeleteRow
{
    public List<UdtRowUidCondition> Conditions { get; init; } = new();
}

public sealed class UdtInsertRequest
{
    public string Table { get; init; } = string.Empty;
    public List<UdtInsertRow> Rows { get; init; } = new();
}

public sealed class UdtUpdateRequest
{
    public string Table { get; init; } = string.Empty;
    public List<UdtUpdateRow> Rows { get; init; } = new();
}

public sealed class UdtDeleteRequest
{
    public string Table { get; init; } = string.Empty;
    public List<UdtDeleteRow> Rows { get; init; } = new();
}

public sealed class UdtException : Exception
{
    public System.Net.HttpStatusCode StatusCode { get; }

    public UdtException(string message, System.Net.HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public sealed class UdtTypeMap
{
    public Type DtoType { get; }
    public string TableName { get; }
    public PropertyInfo UidProperty { get; }
    public UdtUidAttribute UidAttribute { get; }
    public IReadOnlyList<UdtMappedColumn> Columns { get; }

    public UdtTypeMap(Type dtoType, string tableName, PropertyInfo uidProperty, UdtUidAttribute uidAttribute, IReadOnlyList<UdtMappedColumn> columns)
    {
        DtoType = dtoType;
        TableName = tableName;
        UidProperty = uidProperty;
        UidAttribute = uidAttribute;
        Columns = columns;
    }
}

public sealed class UdtMappedColumn
{
    public PropertyInfo Property { get; }
    public UdtColumnAttribute Attribute { get; }

    public UdtMappedColumn(PropertyInfo property, UdtColumnAttribute attribute)
    {
        Property = property;
        Attribute = attribute;
    }
}
