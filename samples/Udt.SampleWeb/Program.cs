using UdtClient;
using UdtClient.SampleWeb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUdtClient(
    options =>
    {
        options.BaseUrl = "https://your-host-name";
        options.InsertPath = "/udtservice/api/udtdata/insertudtdata";
        options.UpdatePath = "/udtservice/api/udtdata/updateudtdata";
        options.DeletePath = "/udtservice/api/udtdata/deleteudtdata";
    },
    udt =>
    {
        udt.AddDto<VendorSyncDto>();
    });

var app = builder.Build();

app.MapGet("/", () => "Prophet21 UDT sample app");

app.MapPost("/vendor-sync", async (IUdtClient client, CancellationToken cancellationToken) =>
{
    var dto = new VendorSyncDto
    {
        RowUid = 14,
        VendorId = "12345",
        BandVendorId = "1",
        LastSynchronizedDate = new DateOnly(2025, 1, 1)
    };

    await client.UpdateAsync(dto, cancellationToken);
    return Results.Ok();
});

app.Run();
