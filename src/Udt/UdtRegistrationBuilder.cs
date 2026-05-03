namespace UdtClient;

internal sealed class UdtRegistrationBuilder : IUdtRegistrationBuilder
{
    private readonly List<Type> _dtoTypes = new();

    public IReadOnlyList<Type> DtoTypes => _dtoTypes;

    public IUdtRegistrationBuilder AddDto<T>()
    {
        _dtoTypes.Add(typeof(T));
        return this;
    }
}
