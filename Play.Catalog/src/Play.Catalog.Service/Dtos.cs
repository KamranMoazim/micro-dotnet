using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public record ItemDto(Guid id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);

    public record CreateItemDto([Required] string Name, string Description, [Range(10, 1000)] decimal Price);

    public record UpdateItemDto([Required] string Name, string Description, [Range(10, 1000)] decimal Price);
}