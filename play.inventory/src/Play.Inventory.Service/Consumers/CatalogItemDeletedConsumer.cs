using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
{

    private readonly IRepostry<CatalogItem> repostry;

    public CatalogItemDeletedConsumer(IRepostry<CatalogItem> repostry)
    {
        this.repostry = repostry;
    }

    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var message = context.Message;
        var item = await repostry.GetAsync(message.ItemId);
        if (item == null)
        {
            return;
        }

        await repostry.DeleteAsync(item.Id);


    }
}