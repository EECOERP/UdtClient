using UdtClient;
using Xunit;

namespace UdtClient.Tests;

public sealed class UdtMapperTests
{
    [Fact]
    public void MapInsert_SingleDto_ReturnsCorrectRequest()
    {
        var registry = new UdtTypeMapRegistry(new ReflectionUdtTypeMapFactory());
        registry.Register<TestDto>();
        var mapper = new ReflectionUdtMetadataMapper(registry);

        var result = mapper.MapInsert(new TestDto
        {
            RowUid = 1,
            VendorId = "V001",
            BandVendorId = "B001"
        });

        Assert.Equal("udt_band_api_sync_vendors", result.Table);
        Assert.Single(result.Rows);
        Assert.Contains(result.Rows[0].Columns, c => c.Name == "vendor_id" && c.Value == "V001");
        Assert.Contains(result.Rows[0].Columns, c => c.Name == "band_vendor_id" && c.Value == "B001");
    }

    [Fact]
    public void MapInsert_MultipleDto_ReturnsAllRows()
    {
        var registry = new UdtTypeMapRegistry(new ReflectionUdtTypeMapFactory());
        registry.Register<TestDto>();
        var mapper = new ReflectionUdtMetadataMapper(registry);

        var result = mapper.MapInsert(new List<TestDto>
        {
            new() { RowUid = 1, VendorId = "V001", BandVendorId = "B001" },
            new() { RowUid = 2, VendorId = "V002", BandVendorId = "B002" }
        });

        Assert.Equal("udt_band_api_sync_vendors", result.Table);
        Assert.Equal(2, result.Rows.Count);
    }

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
    public void MapDelete_SingleDto_Uses_RowUid_In_Conditions()
    {
        var registry = new UdtTypeMapRegistry(new ReflectionUdtTypeMapFactory());
        registry.Register<TestDto>();
        var mapper = new ReflectionUdtMetadataMapper(registry);

        var result = mapper.MapDelete(new TestDto
        {
            RowUid = 7,
            VendorId = "12345",
            BandVendorId = "1"
        });

        Assert.Equal("udt_band_api_sync_vendors", result.Table);
        Assert.Single(result.Rows);
        Assert.Equal(7, result.Rows[0].Conditions[0].RowUid);
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
    private sealed class TestDto : IUdtDto
    {
        [UdtUid]
        public int RowUid { get; init; }

        [UdtColumn("vendor_id")]
        public string VendorId { get; init; } = string.Empty;

        [UdtColumn("band_vendor_id")]
        public string BandVendorId { get; init; } = string.Empty;
    }

    [UdtTable("bad")]
    private sealed class MissingUidDto : IUdtDto
    {
        [UdtColumn("vendor_id")]
        public string VendorId { get; init; } = string.Empty;
    }
}
