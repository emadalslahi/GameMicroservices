using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
{

    private readonly IRepostry<CatalogItem> repostry;

    public CatalogItemUpdatedConsumer(IRepostry<CatalogItem> repostry)
    {
        this.repostry = repostry;
    }

    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var message = context.Message;
        var item = await repostry.GetAsync(message.ItemId);
        if (item == null)
        {
            item = new CatalogItem(message.ItemId, message.Name, message.Description);
            await repostry.CreateAsync(item);
        }
        else
        {
            item.Name = message.Name;
            item.description = message.Description;
            await repostry.UpdateAsync(item);
        }

    }
}