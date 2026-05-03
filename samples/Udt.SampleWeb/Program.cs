using UdtClient;
using UdtClient.SampleWeb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUdtClient(
    options =>
    {
        options.BaseUrl = "https://your-host-name";
        options.BearerToken = "sfsdf";
    },
    udt =>
    {
        udt.AddDto<CustomerInventoryReservation>();
    });

var app = builder.Build();

app.MapGet("/", () => "Prophet21 UDT sample app");

app.MapPost("/vendor-sync", async (IUdtClient client, CancellationToken cancellationToken) =>
{
    var dto = new CustomerInventoryReservation
    {
        RowUid = 14,
        CustomerId = "12345",
        ReservedQuantity = 1
    };

    await client.UpdateAsync(dto, cancellationToken);
    return Results.Ok();
});

app.Run();
