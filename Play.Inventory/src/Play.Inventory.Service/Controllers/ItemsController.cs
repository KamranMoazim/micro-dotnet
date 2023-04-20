using System;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : Controller
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly IRepository<CatalogItem> categoryItemsRepository;


        public ItemsController(
            IRepository<InventoryItem> itemsRepository,
            IRepository<CatalogItem> categoryItemsRepository
        )
        {
            this.itemsRepository = itemsRepository;
            this.categoryItemsRepository = categoryItemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryItemsEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemsEntities.Select(item => item.CatalogItemId);
            var catalogItemsEntities = await categoryItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemsDtos = inventoryItemsEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemsEntities.Single(item => item.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemsDtos);

        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto grantItemDto)
        {
            var inventoryItem = await itemsRepository.GetAsync(
                item => item.UserId == grantItemDto.UserID && item.CatalogItemId == grantItemDto.CatalogItemId
            );

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemDto.CatalogItemId,
                    UserId = grantItemDto.UserID,
                    Quantity = grantItemDto.Quantity,
                    AcquireDate = DateTimeOffset.UtcNow
                };

                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}