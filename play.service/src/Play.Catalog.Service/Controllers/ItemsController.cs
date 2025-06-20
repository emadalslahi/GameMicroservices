using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Entities;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Extensions;
using Play.Common.Repositories;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepostry<Item> _repository;
    private static int requestCount = 0;

    private readonly IPublishEndpoint publishEndpoint;
    public ItemsController(IRepostry<Item> repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
    {

        requestCount++;
        // Log the request count
        Console.WriteLine($"Request Count: {requestCount}");

        var items = (await _repository.GetAllAsync())
            .Select(item => item.AsDto());

        Console.WriteLine($"Request Count: {requestCount} Ok {items.Count()} items");
        // return await _repository.GetItemsAsync()
        //               .ContinueWith(task => task.Result.Select(item => item.AsDto()));
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
    {

        var item = await _repository.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return item.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Description = itemDto.Description,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Validate the itemDto before adding it to the list
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            return BadRequest("Name is required.");
        }
        if (string.IsNullOrWhiteSpace(item.Description) || item.Description.Length < 10 || item.Description.Length > 1000)
        {
            return BadRequest("Description must be between 10 and 1000 characters.");
        }
        if (item.Price <= 0 || item.Price > 1000)
        {
            return BadRequest("Price must be between 0.01 and 1000.");
        }
        // Add the item to the repository
        await _repository.CreateAsync(item);

        // Publish an event after creating the item
        await publishEndpoint.Publish(new Contracts.CatalogItemCreated(item.Id,
             item.Name,
             item.Description));


        return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ItemDto>> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
    {


        var existingItem = await _repository.GetAsync(id);
        if (existingItem is null)
        {
            return NotFound();
        }
        // Validate the itemDto before updating the existing item
        if (string.IsNullOrWhiteSpace(itemDto.Name))
        {
            return BadRequest("Name is required.");
        }
        if (string.IsNullOrWhiteSpace(itemDto.Description) || itemDto.Description.Length < 10 || itemDto.Description.Length > 1000)
        {
            return BadRequest("Description must be between 10 and 1000 characters.");
        }
        if (itemDto.Price <= 0 || itemDto.Price > 1000)
        {
            return BadRequest("Price must be between 0.01 and 1000.");
        }
        // Update the existing item with the new values
        existingItem.Id = id; // Ensure the ID remains the same
        existingItem.Name = itemDto.Name;
        existingItem.Description = itemDto.Description;
        existingItem.Price = itemDto.Price;

        // Update the item in the repository
        await _repository.UpdateAsync(existingItem);
        // Publish an event after updating the item
        await publishEndpoint.Publish(new Contracts.CatalogItemUpdated(existingItem.Id,
             existingItem.Name,
             existingItem.Description));

        return Ok(existingItem);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItemAsync(Guid id)
    {


        var existingItem = await _repository.GetAsync(id);
        // Check if the item exists
        if (existingItem is null)
        {
            return NotFound();
        }

        // Delete the item from the repository
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        // Publish an event after deleting the item
        await publishEndpoint.Publish(new Contracts.CatalogItemDeleted(id));
        return NoContent();
    }
}