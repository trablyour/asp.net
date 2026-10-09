using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos;
using ShopApi.Models;
using ShopApi.Repositories;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ProductsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _uow.Products.GetAllWithCategoryAsync();
            return Ok(products);
        }

        // GET api/products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _uow.Products.GetByIdWithCategoryAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create(ProductDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return BadRequest($"Категория с id {dto.CategoryId} не найдена");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId
            };

            await _uow.Products.AddAsync(product);
            await _uow.SaveAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT api/products/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var category = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return BadRequest($"Категория с id {dto.CategoryId} не найдена");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            _uow.Products.Update(product);
            await _uow.SaveAsync();

            return NoContent();
        }

        // DELETE api/products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            _uow.Products.Delete(product);
            await _uow.SaveAsync();

            return NoContent();
        }
    }
}
