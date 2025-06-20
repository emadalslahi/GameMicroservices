using Play.Common.Entities;

namespace Play.Inventory.Service.Entities;

public class CatalogItem(Guid id, string Name, string description) : IEntity
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = Name;
    public string description { get; set; } = description;
}