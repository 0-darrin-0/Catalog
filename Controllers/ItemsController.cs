using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Linq;
using Catalog.DTOs;
using Catalog.Entities;

namespace Catalog.Controllers
{

    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;
        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        // GET /items
        [HttpGet]
        public IEnumerable<ItemDTO> GetItems() 
        {
            var items = repository.GetItems().Select(item => item.AsDTO());
            return items;
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItem(Guid id)
        {
            var item =  repository.GetItem(id);

            if (item is null)
            {
                return NotFound();
            }

            return item.AsDTO();
        }

        //POST /items
        [HttpPost()]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO itemDTO)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Price = itemDTO.Price,
                CreatedDate = DateTime.UtcNow
            };

            repository.CreateItem(item);

            return CreatedAtAction(nameof(GetItem), new { id = item.Id}, item.AsDTO());
        }
    }
}