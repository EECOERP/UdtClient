using UdtClient;

namespace UdtClient.SampleWeb;

[UdtTable("udt_band_api_sync_vendors")]
public sealed class VendorSyncDto
{
    [UdtUid]
    public int RowUid { get; init; }

    [UdtColumn("vendor_id")]
    public string VendorId { get; init; } = string.Empty;

    [UdtColumn("band_vendor_id")]
    public string BandVendorId { get; init; } = string.Empty;

    [UdtColumn("date_last_synchronized")]
    public DateOnly LastSynchronizedDate { get; init; }
}
