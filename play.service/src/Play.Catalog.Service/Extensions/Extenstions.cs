using Play.Catalog.Service.Dtos;

namespace   Play.Catalog.Service.Extensions;
public static class Extensions
{


    public static ItemDto AsDto(this Entities.Item item)
    {
        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            CreatedDate = item.CreatedDate
        };
    }
    


    public static T? GetValueOrDefault<T>(this T? value, T? defaultValue = default) where T : struct
    {
        return value.HasValue ? value : defaultValue;
    }

    public static string ToJson(this object obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }

    public static T FromJson<T>(this string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Deserialization failed.");
    }
}