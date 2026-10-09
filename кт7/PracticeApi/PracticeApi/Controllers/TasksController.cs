using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Dtos;
using PracticeApi.Models;

namespace PracticeApi.Controllers
{
    // Задание 2. TMS
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TasksController(AppDbContext db)
        {
            _db = db;
        }

        // GET api/tasks
        // GET api/tasks?status=InProgress
        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetAll([FromQuery] TaskItemStatus? status)
        {
            var query = _db.Tasks.AsQueryable();

            if (status != null)
                query = query.Where(t => t.Status == status);

            return await query.OrderBy(t => t.Id).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound("Задача не найдена");

            return task;
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create(CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = TaskItemStatus.New,
                CreatedAt = DateTime.Now
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        // PATCH api/tasks/1/status   { "status": "Done" }
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<TaskItem>> UpdateStatus(int id, UpdateStatusDto dto)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound("Задача не найдена");

            task.Status = dto.Status!.Value;
            await _db.SaveChangesAsync();

            return task;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound("Задача не найдена");

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
