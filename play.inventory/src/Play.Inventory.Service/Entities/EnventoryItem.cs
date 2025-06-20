using Play.Common.Entities;

namespace Play.Inventory.Service.Entities;

public class InventoryItem :IEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CatalogItemId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset AcquiredDate { get; set; }

    public InventoryItem(Guid userId, Guid catalogItemId, int quantity, DateTimeOffset acquiredDate)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CatalogItemId = catalogItemId;
        Quantity = quantity;
        AcquiredDate = acquiredDate;
    }
}