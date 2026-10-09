using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Dtos;
using PracticeApi.Models;

namespace PracticeApi.Controllers
{
    // Задание 1. Список студентов
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StudentsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetAll()
        {
            return await _db.Students.OrderBy(s => s.LastName).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetById(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound("Студент не найден");

            return student;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> Create(StudentDto dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Group = dto.Group,
                Course = dto.Course,
                Email = dto.Email
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StudentDto dto)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound("Студент не найден");

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Group = dto.Group;
            student.Course = dto.Course;
            student.Email = dto.Email;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound("Студент не найден");

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
