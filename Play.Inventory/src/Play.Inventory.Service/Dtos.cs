using System;

namespace Play.Inventory.Service.Dtos
{

    public record GrantItemDto(Guid UserID, Guid CatalogItemId, int Quantity);

    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquireDate);

    public record CatalogItemDto(Guid id, string Name, string Description);
}