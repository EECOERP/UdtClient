# UdtClient

A lightweight, strongly-typed .NET client for working with User Defined Tables (UDTs) via HTTP APIs.

This library removes the need to manually construct JSON payloads by allowing you to use simple C# DTOs with attributes.

## Why this exists

Working with UDT APIs typically requires building JSON payloads manually. This is:

- error-prone  
- repetitive  
- hard to maintain  

**UdtClient replaces that with strongly-typed C# DTOs**, making your code cleaner and safer.

## Quick Start

### 1. Register your POCO in Program.cs:

```csharp
builder.Services.AddUdtClient(
    options =>
    {
        options.BaseUrl = "https://your-host-name";
    },
    udt =>
    {
        udt.AddDto<CustomerInventoryReservationDto>();
    });
```

### 2. Decorate any POCO with attributes to map your class and properties to your UDT table and columns: 
```csharp
[UdtTable("udt_customer_inventory_reservation")]
public sealed class CustomerInventoryReservationDto
{
    [UdtUid]
    public int RowUid { get; init; }

    [UdtColumn("customer_id")]
    public int CustomerId { get; init; }

    [UdtColumn("item_id")]
    public string ItemId { get; init; } = "";

    [UdtColumn("reserved_qty")]
    public int ReservedQuantity { get; init; } 
}
```

### 3. Insert, Update, or Delete single instance or lists of your registered and mapped POCO:

```csharp
var reservation = new CustomerInventoryReservation
{
    CustomerId = formModel.CustomerId,
    ItemId = formModel.ItemId,
    ReservedQuantity = formModel.ReservedQuantity
};

await UdtRepository.InsertAsync<CustomerInventoryReservation>(reservation);
```


## Design Principles

- Explicit DTO registration (no automatic scanning)
- Strong validation at startup
- Exactly one UID per DTO
- No dynamic or string-based mapping
- Focused on UDT operations only (not an ORM)

## Disclaimer

This project is not affiliated with, endorsed by, or supported by Epicor Software Corporation.

“Epicor” and “Prophet 21” are trademarks of Epicor Software Corporation. This library is an independent tool designed to work with compatible UDT APIs.

