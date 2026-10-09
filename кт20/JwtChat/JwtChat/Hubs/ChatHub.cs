using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using JwtChat.Data;
using JwtChat.Models;
using JwtChat.Services;

namespace JwtChat.Hubs
{
    // 3-4. подключиться можно только с валидным JWT токеном
    [Authorize]
    public class ChatHub : Hub
    {
        private const int MaxLength = 500;
        private const int HistorySize = 50;

        private readonly AppDbContext _db;
        private readonly ConnectionTracker _tracker;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(AppDbContext db, ConnectionTracker tracker, ILogger<ChatHub> logger)
        {
            _db = db;
            _tracker = tracker;
            _logger = logger;
        }

        // имя берется из проверенного токена, клиент не может его подменить
        private string UserName => Context.User?.Identity?.Name ?? "unknown";

        private int UserId => int.TryParse(Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out int id) ? id : 0;

        public override async Task OnConnectedAsync()
        {
            bool joined = _tracker.Add(UserName, Context.ConnectionId);
            _logger.LogInformation("Подключился {User}", UserName);

            if (joined)
                await Clients.Others.SendAsync("UserJoined", UserName);

            await Clients.All.SendAsync("OnlineUsers", _tracker.OnlineUsers());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            bool left = _tracker.Remove(UserName, Context.ConnectionId);
            _logger.LogInformation("Отключился {User}", UserName);

            if (left)
                await Clients.Others.SendAsync("UserLeft", UserName);

            await Clients.All.SendAsync("OnlineUsers", _tracker.OnlineUsers());
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<MessageDto>> GetHistory()
        {
            var messages = await _db.Messages
                .OrderByDescending(m => m.SentAt)
                .Take(HistorySize)
                .ToListAsync();

            return messages.OrderBy(m => m.SentAt).Select(ToDto).ToList();
        }

        public async Task SendMessage(string text)
        {
            EnsureTokenNotExpired();

            text = (text ?? "").Trim();
            if (text.Length == 0)
                return;

            if (text.Length > MaxLength)
                throw new HubException($"Сообщение длиннее {MaxLength} символов");

            var message = new ChatMessage
            {
                UserId = UserId,
                UserName = UserName,
                Text = text,
                SentAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", ToDto(message));
        }

        public Task Typing()
        {
            return Clients.Others.SendAsync("UserTyping", UserName);
        }

        // только администратор может очистить историю
        [Authorize(Roles = Roles.Admin)]
        public async Task ClearChat()
        {
            EnsureTokenNotExpired();

            await _db.Messages.ExecuteDeleteAsync();
            _logger.LogInformation("Чат очищен администратором {User}", UserName);

            await Clients.All.SendAsync("ChatCleared", UserName);
        }

        // токен проверяется при подключении, но соединение может жить дольше токена.
        // поэтому срок проверяем еще и при отправке
        private void EnsureTokenNotExpired()
        {
            string? expValue = Context.User?.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

            if (long.TryParse(expValue, out long exp) &&
                DateTimeOffset.FromUnixTimeSeconds(exp) <= DateTimeOffset.UtcNow)
            {
                Context.Abort();
                throw new HubException("Срок действия токена истек, войдите заново");
            }
        }

        private static MessageDto ToDto(ChatMessage m) =>
            new MessageDto(m.UserName, m.Text, DateTime.SpecifyKind(m.SentAt, DateTimeKind.Utc));
    }
}
