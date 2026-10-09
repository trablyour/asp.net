using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SecureChat.Data;
using SecureChat.Models;
using SecureChat.Services;

namespace SecureChat.Hubs
{
    // [Authorize]: без куки входа подключиться к хабу нельзя
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

        // имя берем из куки аутентификации, а не от клиента - подделать нельзя
        private string UserName => Context.User?.Identity?.Name ?? "unknown";

        private int UserId => int.TryParse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 0;

        public override async Task OnConnectedAsync()
        {
            bool justJoined = _tracker.Add(UserName, Context.ConnectionId);
            _logger.LogInformation("Подключился {User} ({Connection})", UserName, Context.ConnectionId);

            if (justJoined)
                await Clients.Others.SendAsync("UserJoined", UserName);

            await Clients.All.SendAsync("OnlineUsers", _tracker.OnlineUsers());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            bool left = _tracker.Remove(UserName, Context.ConnectionId);
            _logger.LogInformation("Отключился {User} ({Connection})", UserName, Context.ConnectionId);

            if (left)
                await Clients.Others.SendAsync("UserLeft", UserName);

            await Clients.All.SendAsync("OnlineUsers", _tracker.OnlineUsers());
            await base.OnDisconnectedAsync(exception);
        }

        // история последних сообщений
        public async Task<List<MessageDto>> GetHistory()
        {
            var messages = await _db.Messages
                .OrderByDescending(m => m.SentAt)
                .Take(HistorySize)
                .ToListAsync();

            return messages
                .OrderBy(m => m.SentAt)
                .Select(ToDto)
                .ToList();
        }

        public async Task SendMessage(string text)
        {
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

        // "пользователь печатает..."
        public Task Typing()
        {
            return Clients.Others.SendAsync("UserTyping", UserName);
        }

        // SQLite не хранит тип даты, явно помечаем как UTC, чтобы браузер правильно перевел в местное время
        private static MessageDto ToDto(ChatMessage m) =>
            new MessageDto(m.UserName, m.Text, DateTime.SpecifyKind(m.SentAt, DateTimeKind.Utc));
    }
}
