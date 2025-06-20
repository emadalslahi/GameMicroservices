using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients;

public class CatalogClient
{
    private readonly HttpClient _httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemAsync()
    {
        var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>($"/items");
        return items ?? new List<CatalogItemDto>();
    }
    public async Task<CatalogItemDto> GetCatalogItemAsync(Guid id)
    {
        var item = await _httpClient.GetFromJsonAsync<CatalogItemDto>($"/items/{id}");
        return item ?? throw new Exception($"Catalog item with id {id} not found.");
    }
}