using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos;

public record ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = string.Empty;
    [Range(0.01, 1000, ErrorMessage = "Price must be between 0.01 and 1000.")]
    [Required(ErrorMessage = "Price is required.")]
    public decimal Price { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
public record CreateItemDto
{

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; init; } = string.Empty;
    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
    [RegularExpression(@"^[a-zA-Z0-9\s,.!?-]+$", ErrorMessage = "Description can only contain letters, numbers, spaces, and basic punctuation.")]

    public string Description { get; init; } = string.Empty;
    [Range(0.01, 1000, ErrorMessage = "Price must be between 0.01 and 1000.")]
    [Required(ErrorMessage = "Price is required.")]
    public decimal Price { get; init; }
}
public record UpdateItemDto
{

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; init; } = string.Empty;
    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
    [RegularExpression(@"^[a-zA-Z0-9\s,.!?-]+$", ErrorMessage = "Description can only contain letters, numbers, spaces, and basic punctuation.")]

    public string Description { get; init; } = string.Empty;
    [Range(0.01, 1000, ErrorMessage = "Price must be between 0.01 and 1000.")]
    [Required(ErrorMessage = "Price is required.")]
    public decimal Price { get; init; }
}