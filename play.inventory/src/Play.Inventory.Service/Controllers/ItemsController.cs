using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> _logger;
    //private readonly InventoryService _inventoryService;

    private readonly CatalogClient _catalogClient;

    private readonly IRepostry<CatalogItem> _catalogItemRepository;
    private readonly IRepostry<Entities.InventoryItem> _repository;

    public ItemsController(ILogger<ItemsController> logger,
                           IRepostry<Entities.InventoryItem> repository,
                           CatalogClient catalogClient,
                           IRepostry<CatalogItem> catalogItemRepository)
    {
        _logger = logger;
        _repository = repository;
        _catalogClient = catalogClient;
        _catalogItemRepository = catalogItemRepository;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        _logger.LogInformation("Received request to get items for user {UserId}", userId);
        if (userId == Guid.Empty)
        {
            return BadRequest("UserId cannot be empty.");
        }
        _logger.LogInformation("Getting items for user {UserId}", userId);

        var InvtoryItemEntities = await _repository.GetAllAsync(item => item.UserId == userId);
        var CtlgItemIds = InvtoryItemEntities.Select(item => item.CatalogItemId);
        var catalogItems = await _catalogItemRepository.GetAllAsync(item => CtlgItemIds.Contains(item.Id));
        var inventoryItemDtos = InvtoryItemEntities.Select(inventryItem =>
        {
            var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventryItem.CatalogItemId);
            return inventryItem.AsDto(catalogItem.Name, catalogItem.description);
        });
        return Ok(inventoryItemDtos);


    }
    [HttpGet]
    [Route("/withClint")]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetWithClintsAsync(Guid userId)
    {
        _logger.LogInformation("Received request to get items for user {UserId}", userId);
        if (userId == Guid.Empty)
        {
            return BadRequest("UserId cannot be empty.");
        }
        _logger.LogInformation("Getting items for user {UserId}", userId);
        var catalogItems = await _catalogClient.GetCatalogItemAsync();
        if (catalogItems == null || !catalogItems.Any())
        {
            _logger.LogWarning("No catalog items found.");
            return NotFound("No catalog items found.");
        }
        var InvtoryItemEntities = await _repository.GetAllAsync(item => item.UserId == userId);
        var inventoryItemDtos = InvtoryItemEntities.Select(inventryItem =>
        {
            var catalogItem = catalogItems.Single(catalogItem => catalogItem.id == inventryItem.CatalogItemId);
            return inventryItem.AsDto(catalogItem.name, catalogItem.description);
        });
        return Ok(inventoryItemDtos);


    }


    [HttpPost()]
    public async Task<ActionResult> GrantItemAsync(GranntItemsDto granntItemsDto)
    {

        _logger.LogInformation("Granting item to user {UserId} for CatalogItemId {CatalogItemId} with Quantity {Quantity}",
            granntItemsDto.UserId, granntItemsDto.CatalogItemId, granntItemsDto.Quantity);



        if (granntItemsDto.UserId == Guid.Empty)
        {
            return BadRequest("UserId cannot be empty.");
        }
        if (granntItemsDto.CatalogItemId == Guid.Empty)
        {
            return BadRequest("CatalogItemId cannot be empty.");
        }
        if (granntItemsDto.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than zero.");
        }

        _logger.LogInformation("Granting {Quantity} of item {CatalogItemId} to user {UserId}",
            granntItemsDto.Quantity, granntItemsDto.CatalogItemId, granntItemsDto.UserId);



        var existingItem = (await _repository.GetAllAsync(item => item.UserId == granntItemsDto.UserId &&
                                                              item.CatalogItemId == granntItemsDto.CatalogItemId)).FirstOrDefault();
        var catalogItem = await _catalogClient.GetCatalogItemAsync(granntItemsDto.CatalogItemId);
        if (catalogItem == null)
        {
            _logger.LogWarning("Catalog item with id {CatalogItemId} not found.", granntItemsDto.CatalogItemId);
            return NotFound($"Catalog item with id {granntItemsDto.CatalogItemId} not found.");
        }
        _logger.LogInformation("Catalog item found: {CatalogItemName} with description {CatalogItemDescription}",
            catalogItem.name, catalogItem.description);

        if (existingItem != null)
        {
            existingItem.Quantity += granntItemsDto.Quantity;
            await _repository.UpdateAsync(existingItem);
            return Ok(existingItem.AsDto(catalogItem.name, catalogItem.description));
        }
        else
        {
            var newItem = new InventoryItem(granntItemsDto.UserId,
                                            granntItemsDto.CatalogItemId,
                                            granntItemsDto.Quantity,
                                            DateTimeOffset.UtcNow);
            await _repository.CreateAsync(newItem);

            return CreatedAtAction(nameof(GrantItemAsync), new { userId = granntItemsDto.UserId }, newItem.AsDto(catalogItem.name, catalogItem.description));
        }


    }


}