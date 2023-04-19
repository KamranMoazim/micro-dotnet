using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemControllers : ControllerBase
    {
        private static readonly List<ItemDto> items = new List<ItemDto>()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restore small potion of hp", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cure poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals small amount of damage", 20, DateTimeOffset.UtcNow)
        };


        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }


        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.Where(item => item.id == id).SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            return item!;
        }


        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.id }, item);
        }


        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = items.Where(item => item.id == id).SingleOrDefault();

            if (existingItem == null)
            {
                return NotFound();
            }

            var updatedItem = existingItem! with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };
            var index = items.FindIndex(existingItem => existingItem.id == id);
            items[index] = updatedItem;

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var index = items.FindIndex(existingItem => existingItem.id == id);

            if (index < 0)
            {
                return NotFound();
            }

            items.RemoveAt(index);

            return NoContent();
        }



    }
}