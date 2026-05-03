using UdtClient;
using Xunit;

namespace UdtClient.Tests;

public sealed class UdtMapperTests
{
    [Fact]
    public void MapUpdate_Uses_RowUid_In_Conditions()
    {
        var registry = new UdtTypeMapRegistry(new ReflectionUdtTypeMapFactory());
        registry.Register<TestDto>();
        var mapper = new ReflectionUdtMetadataMapper(registry);

        var result = mapper.MapUpdate(new TestDto
        {
            RowUid = 14,
            VendorId = "12345",
            BandVendorId = "1"
        });

        Assert.Equal("udt_band_api_sync_vendors", result.Table);
        Assert.Single(result.Rows);
        Assert.Single(result.Rows[0].Conditions);
        Assert.Equal(14, result.Rows[0].Conditions[0].RowUid);
    }

    [Fact]
    public void Factory_Rejects_Dto_Without_Uid()
    {
        var factory = new ReflectionUdtTypeMapFactory();
        Assert.Throws<InvalidOperationException>(() => factory.Create(typeof(MissingUidDto)));
    }

    [Fact]
    public void MapDelete_Rejects_Non_Positive_Uid()
    {
        var registry = new UdtTypeMapRegistry(new ReflectionUdtTypeMapFactory());
        registry.Register<TestDto>();
        var mapper = new ReflectionUdtMetadataMapper(registry);

        Assert.Throws<InvalidOperationException>(() => mapper.MapDelete(new TestDto
        {
            RowUid = 0,
            VendorId = "12345",
            BandVendorId = "1"
        }));
    }

    [UdtTable("udt_band_api_sync_vendors")]
    private sealed class TestDto
    {
        [UdtUid]
        public int RowUid { get; init; }

        [UdtColumn("vendor_id")]
        public string VendorId { get; init; } = string.Empty;

        [UdtColumn("band_vendor_id")]
        public string BandVendorId { get; init; } = string.Empty;
    }

    [UdtTable("bad")]
    private sealed class MissingUidDto
    {
        [UdtColumn("vendor_id")]
        public string VendorId { get; init; } = string.Empty;
    }
}
