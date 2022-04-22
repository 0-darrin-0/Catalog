using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Linq;
using Catalog.DTOs;
using Catalog.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{

    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        private readonly ILogger<ItemsController> logger;

        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync() 
        {
            var items = (await repository.GetItemsAsync())
                        .Select(item => item.AsDTO());
            
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {items.Count()} items");

            return items;
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemAsync(Guid id)
        {
            var item =  await repository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            return item.AsDTO();
        }

        //POST /items
        [HttpPost()]
        public async Task<ActionResult<ItemDTO>> CreateItemAsync(CreateItemDTO itemDTO)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Price = itemDTO.Price,
                CreatedDate = DateTime.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id}, item.AsDTO());
        }

        //PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDTO itemDTO)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with
            {
                Name = itemDTO.Name,
                Price = itemDTO.Price,
            };

            await repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        //DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}