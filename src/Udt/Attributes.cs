using System;

namespace UdtClient;

/// <summary>
/// Specifies the UDT table name this DTO maps to. Required on all <see cref="IUdtDto"/> types.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class UdtTableAttribute : Attribute
{
    public string TableName { get; }

    public UdtTableAttribute(string tableName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        TableName = tableName;
    }
}

/// <summary>
/// Marks the property that holds the row UID. Exactly one property per DTO must carry this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class UdtUidAttribute : Attribute
{
    /// <summary>
    /// When <see langword="true"/>, the UID value is included as a column in insert requests.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool IncludeInInsertColumns { get; init; } = false;
}

/// <summary>
/// Maps a DTO property to a UDT column name.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class UdtColumnAttribute : Attribute
{
    public string ColumnName { get; }

    /// <summary>
    /// When <see langword="true"/>, the column is omitted from the request if the property value is <see langword="null"/>.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool IgnoreIfNull { get; init; } = false;

    public UdtColumnAttribute(string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        ColumnName = columnName;
    }
}
