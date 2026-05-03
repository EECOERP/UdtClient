using System;

namespace UdtClient;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UdtTableAttribute : Attribute
{
    public string TableName { get; }

    public UdtTableAttribute(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be empty.", nameof(tableName));

        TableName = tableName;
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class UdtColumnAttribute : Attribute
{
    public string ColumnName { get; }
    public bool IgnoreIfNull { get; set; }

    public UdtColumnAttribute(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            throw new ArgumentException("Column name cannot be empty.", nameof(columnName));

        ColumnName = columnName;
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class UdtUidAttribute : Attribute
{
    public bool IncludeInInsertColumns { get; set; }
}
