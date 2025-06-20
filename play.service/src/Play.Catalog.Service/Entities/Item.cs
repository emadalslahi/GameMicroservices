using Play.Common.Entities;

namespace Play.Catalog.Entities;

public class Item : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    // Additional properties can be added as needed
}
// This class represents an item in the catalog with properties such as Id, Name, Description, Price, and CreatedDate.
// It can be extended with additional properties as needed for the catalog service.
// The Id is initialized with a new GUID by default, ensuring each item has a unique identifier.
// The CreatedDate is set to the current UTC time when the item is created, providing a timestamp for when the item was added to the catalog.
// This class can be used in the Play.Catalog.Service to manage items in the catalog, including creating, updating, and retrieving items.