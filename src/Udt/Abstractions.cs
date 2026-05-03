namespace UdtClient;

public interface IUdtClient
{
    Task InsertAsync<T>(T dto, CancellationToken cancellationToken = default);
    Task InsertAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default);
    Task UpdateAsync<T>(T dto, CancellationToken cancellationToken = default);
    Task UpdateAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default);
    Task DeleteAsync<T>(T dto, CancellationToken cancellationToken = default);
    Task DeleteAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default);
}

public interface IUdtMetadataMapper
{
    UdtInsertRequest MapInsert<T>(T dto);
    UdtInsertRequest MapInsert<T>(IEnumerable<T> dtos);
    UdtUpdateRequest MapUpdate<T>(T dto);
    UdtUpdateRequest MapUpdate<T>(IEnumerable<T> dtos);
    UdtDeleteRequest MapDelete<T>(T dto);
    UdtDeleteRequest MapDelete<T>(IEnumerable<T> dtos);
}

public interface IUdtTypeMapFactory
{
    UdtTypeMap Create(Type dtoType);
}

public interface IUdtTypeMapRegistry
{
    void Register(Type dtoType);
    void Register<T>();
    UdtTypeMap Get(Type dtoType);
    UdtTypeMap Get<T>();
}

public interface IUdtRegistrationBuilder
{
    IUdtRegistrationBuilder AddDto<T>();
}
