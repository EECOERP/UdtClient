namespace UdtClient;

/// <summary>
/// Marker interface that all UDT DTO types must implement.
/// Provides compile-time verification that only registered UDT types are used.
/// </summary>
public interface IUdtDto { }

public interface IUdtClient
{
    Task InsertAsync<T>(T dto, CancellationToken cancellationToken = default) where T : class;
    Task InsertAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default) where T : class;
    Task UpdateAsync<T>(T dto, CancellationToken cancellationToken = default) where T : class;
    Task UpdateAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default) where T : class;
    Task DeleteAsync<T>(T dto, CancellationToken cancellationToken = default) where T : class;
    Task DeleteAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default) where T : class;
}

public interface IUdtMetadataMapper
{
    UdtInsertRequest MapInsert<T>(T dto) where T : class;
    UdtInsertRequest MapInsert<T>(IEnumerable<T> dtos) where T : class;
    UdtUpdateRequest MapUpdate<T>(T dto) where T : class;
    UdtUpdateRequest MapUpdate<T>(IEnumerable<T> dtos) where T : class;
    UdtDeleteRequest MapDelete<T>(T dto) where T : class;
    UdtDeleteRequest MapDelete<T>(IEnumerable<T> dtos) where T : class;
}

public interface IUdtTypeMapFactory
{
    UdtTypeMap Create(Type dtoType);
}

public interface IUdtTypeMapRegistry
{
    void Register(Type dtoType);
    void Register<T>() where T : class;
    UdtTypeMap Get(Type dtoType);
    UdtTypeMap Get<T>() where T : class;
}

public interface IUdtRegistrationBuilder
{
    IUdtRegistrationBuilder AddDto<T>() where T : class;
}
