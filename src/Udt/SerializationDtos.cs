using System.Text.Json.Serialization;

namespace UdtClient;

internal sealed class InsertUdtDataRequestDto
{
    [JsonPropertyName("table")]
    public string Table { get; set; } = string.Empty;

    [JsonPropertyName("rows")]
    public List<InsertUdtRowDto> Rows { get; set; } = new();
}

internal sealed class InsertUdtRowDto
{
    [JsonPropertyName("columns")]
    public List<InsertUdtColumnDto> Columns { get; set; } = new();

    [JsonPropertyName("conditions")]
    public List<object> Conditions { get; set; } = new();
}

internal sealed class UpdateUdtDataRequestDto
{
    [JsonPropertyName("table")]
    public string Table { get; set; } = string.Empty;

    [JsonPropertyName("rows")]
    public List<UpdateUdtRowDto> Rows { get; set; } = new();
}

internal sealed class UpdateUdtRowDto
{
    [JsonPropertyName("columns")]
    public List<InsertUdtColumnDto> Columns { get; set; } = new();

    [JsonPropertyName("conditions")]
    public List<UdtRowUidConditionDto> Conditions { get; set; } = new();
}

internal sealed class DeleteUdtDataRequestDto
{
    [JsonPropertyName("table")]
    public string Table { get; set; } = string.Empty;

    [JsonPropertyName("rows")]
    public List<DeleteUdtRowDto> Rows { get; set; } = new();
}

internal sealed class DeleteUdtRowDto
{
    [JsonPropertyName("conditions")]
    public List<UdtRowUidConditionDto> Conditions { get; set; } = new();
}

internal sealed class InsertUdtColumnDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

internal sealed class UdtRowUidConditionDto
{
    [JsonPropertyName("rowuid")]
    public int RowUid { get; set; }
}
