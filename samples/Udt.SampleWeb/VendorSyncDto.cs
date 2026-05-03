using UdtClient;

namespace UdtClient.SampleWeb;

[UdtTable("udt_customer_inventory_reservation")]
public sealed class CustomerInventoryReservation
{
    [UdtUid]
    public int RowUid { get; init; }

    [UdtColumn("customer_id")]
    public string CustomerId { get; init; } = string.Empty;

    [UdtColumn("reserved_quantity")]
    public int ReservedQuantity { get; init; }
}
