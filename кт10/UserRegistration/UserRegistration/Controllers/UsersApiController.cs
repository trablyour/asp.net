using Microsoft.AspNetCore.Mvc;
using UserRegistration.Dtos;
using UserRegistration.Services;

namespace UserRegistration.Controllers
{
    // Задание 2. RESTful API
    [ApiController]
    [Route("api/users")]
    public class UsersApiController : ControllerBase
    {
        private readonly UserService _users;

        public UsersApiController(UserService users)
        {
            _users = users;
        }

        // GET api/users
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var users = await _users.GetAllAsync();
            return users.Select(UserResponse.From).ToList();
        }

        // GET api/users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponse>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Id должен быть положительным числом" });

            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = $"Пользователь с id {id} не найден" });

            return UserResponse.From(user);
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
        {
            if (await _users.UsernameTakenAsync(request.Username))
                return Conflict(new { error = "Пользователь с таким именем уже существует" });

            if (await _users.EmailTakenAsync(request.Email))
                return Conflict(new { error = "Этот email уже зарегистрирован" });

            var user = await _users.CreateAsync(request.Username, request.Email, request.Password);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, UserResponse.From(user));
        }

        // PUT api/users/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponse>> Update(int id, UpdateUserRequest request)
        {
            if (id <= 0)
                return BadRequest(new { error = "Id должен быть положительным числом" });

            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = $"Пользователь с id {id} не найден" });

            if (await _users.UsernameTakenAsync(request.Username, id))
                return Conflict(new { error = "Пользователь с таким именем уже существует" });

            if (await _users.EmailTakenAsync(request.Email, id))
                return Conflict(new { error = "Этот email уже зарегистрирован" });

            await _users.UpdateAsync(user, request.Username, request.Email, request.Password);

            return UserResponse.From(user);
        }

        // DELETE api/users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Id должен быть положительным числом" });

            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = $"Пользователь с id {id} не найден" });

            await _users.DeleteAsync(user);
            return NoContent();
        }
    }
}
