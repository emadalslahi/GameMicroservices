namespace Play.Inventory.Service.Dtos;

public record GranntItemsDto(
    Guid UserId,
    Guid CatalogItemId,
    int Quantity);

public record InventoryItemDto(
Guid catalogItemId, string Name , string description, int quantity, DateTimeOffset AquiredDate);

public record CatalogItemDto(Guid id, string name, string description);