using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JwtChat.Data;
using JwtChat.Dtos;
using JwtChat.Models;
using JwtChat.Services;

namespace JwtChat.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokens;
        private readonly ILogger<AuthController> _logger;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public AuthController(AppDbContext db, TokenService tokens, ILogger<AuthController> logger)
        {
            _db = db;
            _tokens = tokens;
            _logger = logger;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Username == request.Username))
                return Conflict(new { error = "Этот логин уже занят" });

            var user = new User
            {
                Username = request.Username,
                // первый пользователь системы - администратор
                Role = await _db.Users.AnyAsync() ? Roles.User : Roles.Admin,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Регистрация: {User} ({Role})", user.Username, user.Role);

            var (token, expires) = _tokens.CreateToken(user);
            return Ok(new AuthResponse(token, user.Username, user.Role, expires));
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { error = "Неверный логин или пароль" });
            }

            _logger.LogInformation("Вход: {User}", user.Username);

            var (token, expires) = _tokens.CreateToken(user);
            return Ok(new AuthResponse(token, user.Username, user.Role, expires));
        }

        // GET api/auth/me - защищенный метод: без токена вернет 401
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            long exp = long.TryParse(User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value, out long e) ? e : 0;

            return Ok(new
            {
                userName = User.Identity?.Name,
                isAdmin = User.IsInRole(Roles.Admin),
                expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        // GET api/auth/admin - только для роли Admin (без нужной роли вернет 403)
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("admin")]
        public async Task<IActionResult> AdminStats()
        {
            return Ok(new
            {
                users = await _db.Users.CountAsync(),
                messages = await _db.Messages.CountAsync()
            });
        }
    }
}
