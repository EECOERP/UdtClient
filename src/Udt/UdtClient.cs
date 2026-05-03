using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace UdtClient;

public sealed class UdtClient : IUdtClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly IUdtMetadataMapper _mapper;
    private readonly UdtOptions _options;

    public UdtClient(HttpClient httpClient, IUdtMetadataMapper mapper, IOptions<UdtOptions> options)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            throw new ArgumentException("BaseUrl is required.", nameof(options));
    }

    public Task InsertAsync<T>(T dto, CancellationToken cancellationToken = default)
        => InsertAsync(_mapper.MapInsert(dto), cancellationToken);

    public Task InsertAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default)
        => InsertAsync(_mapper.MapInsert(dtos), cancellationToken);

    public Task UpdateAsync<T>(T dto, CancellationToken cancellationToken = default)
        => UpdateAsync(_mapper.MapUpdate(dto), cancellationToken);

    public Task UpdateAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default)
        => UpdateAsync(_mapper.MapUpdate(dtos), cancellationToken);

    public Task DeleteAsync<T>(T dto, CancellationToken cancellationToken = default)
        => DeleteAsync(_mapper.MapDelete(dto), cancellationToken);

    public Task DeleteAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default)
        => DeleteAsync(_mapper.MapDelete(dtos), cancellationToken);

    private async Task InsertAsync(UdtInsertRequest request, CancellationToken cancellationToken)
    {
        ApplyAuthenticationHeaders();

        var dto = new InsertUdtDataRequestDto
        {
            Table = request.Table,
            Rows = request.Rows.Select(row => new InsertUdtRowDto
            {
                Columns = row.Columns.Select(column => new InsertUdtColumnDto
                {
                    Name = column.Name,
                    Value = column.Value
                }).ToList(),
                Conditions = row.Conditions
            }).ToList()
        };

        using var response = await _httpClient.PostAsJsonAsync(BuildUrl(_options.InsertPath), dto, SerializerOptions, cancellationToken);
        await EnsureSuccessAsync(response, "insert", cancellationToken);
    }

    private async Task UpdateAsync(UdtUpdateRequest request, CancellationToken cancellationToken)
    {
        ApplyAuthenticationHeaders();

        var dto = new UpdateUdtDataRequestDto
        {
            Table = request.Table,
            Rows = request.Rows.Select(row => new UpdateUdtRowDto
            {
                Columns = row.Columns.Select(column => new InsertUdtColumnDto
                {
                    Name = column.Name,
                    Value = column.Value
                }).ToList(),
                Conditions = row.Conditions.Select(condition => new UdtRowUidConditionDto
                {
                    RowUid = condition.RowUid
                }).ToList()
            }).ToList()
        };

        using var response = await _httpClient.PostAsJsonAsync(BuildUrl(_options.UpdatePath), dto, SerializerOptions, cancellationToken);
        await EnsureSuccessAsync(response, "update", cancellationToken);
    }

    private async Task DeleteAsync(UdtDeleteRequest request, CancellationToken cancellationToken)
    {
        ApplyAuthenticationHeaders();

        var dto = new DeleteUdtDataRequestDto
        {
            Table = request.Table,
            Rows = request.Rows.Select(row => new DeleteUdtRowDto
            {
                Conditions = row.Conditions.Select(condition => new UdtRowUidConditionDto
                {
                    RowUid = condition.RowUid
                }).ToList()
            }).ToList()
        };

        using var response = await _httpClient.PostAsJsonAsync(BuildUrl(_options.DeletePath), dto, SerializerOptions, cancellationToken);
        await EnsureSuccessAsync(response, "delete", cancellationToken);
    }

    private string BuildUrl(string path)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var cleanedPath = path.StartsWith('/') ? path : "/" + path;
        return baseUrl + cleanedPath;
    }

    private void ApplyAuthenticationHeaders()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(_options.BearerToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.BearerToken);
        }

        const string apiKeyHeader = "X-API-Key";
        if (_httpClient.DefaultRequestHeaders.Contains(apiKeyHeader))
        {
            _httpClient.DefaultRequestHeaders.Remove(apiKeyHeader);
        }

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add(apiKeyHeader, _options.ApiKey);
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new UdtException(
            $"UDT {operation} failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}). Response: {body}",
            response.StatusCode);
    }
}
