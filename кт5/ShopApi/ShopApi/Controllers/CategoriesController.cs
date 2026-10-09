using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos;
using ShopApi.Models;
using ShopApi.Repositories;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CategoriesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _uow.Categories.GetAllAsync();
            return Ok(categories);
        }

        // GET api/categories/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _uow.Categories.GetByIdWithProductsAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> Create(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _uow.Categories.AddAsync(category);
            await _uow.SaveAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // PUT api/categories/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category.Name = dto.Name;
            category.Description = dto.Description;

            _uow.Categories.Update(category);
            await _uow.SaveAsync();

            return NoContent();
        }

        // DELETE api/categories/1 (товары категории удалятся каскадно)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            _uow.Categories.Delete(category);
            await _uow.SaveAsync();

            return NoContent();
        }
    }
}
