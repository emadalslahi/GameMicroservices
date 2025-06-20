using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{

    private readonly IRepostry<CatalogItem> repostry;

    public CatalogItemCreatedConsumer(IRepostry<CatalogItem> repostry)
    {
        this.repostry = repostry;
    }

    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;
        var item = await repostry.GetAsync(message.ItemId);
        if (item != null)
        {
            return;
        }
        item = new CatalogItem(message.ItemId, message.Name, message.Description);
        await repostry.CreateAsync(item);
    }
}