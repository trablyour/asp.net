# JwtChat — защищённый чат на SignalR с JWT-токенами

Чат в реальном времени на **ASP.NET Core** и **SignalR** с аутентификацией по **JWT-токенам**. Доступ к хабу есть только у пользователей с действительным токеном.

## Возможности

- регистрация и вход с выдачей JWT-токена;
- чат в реальном времени: история последних 50 сообщений, список пользователей в сети, уведомления «вошёл / вышел», индикатор «печатает...», автоматическое переподключение;
- роли в токене: первый зарегистрированный пользователь — **Admin**, остальные — **User**; очистка чата доступна только администратору;
- таймер до истечения токена, автоматический выход по истечении;
- панель с содержимым токена и проверкой `GET /api/auth/me`;
- Swagger с кнопкой **Authorize** для проверки API с токеном.

## Технологии

.NET 7 / 8 · ASP.NET Core Web API · SignalR · JWT Bearer · Entity Framework Core · SQLite · JavaScript

## Запуск

```bash
dotnet run
```

| Адрес | Что там |
|---|---|
| http://localhost:5000 | чат (вход, регистрация) |
| http://localhost:5000/swagger | Swagger API |

Настройки токена — в `appsettings.json`:

```json
"Jwt": {
  "Issuer": "JwtChat",
  "Audience": "JwtChatClient",
  "Key": "...",
  "ExpiresMinutes": 60
}
```

## API

| Метод | Адрес | Доступ | Описание |
|---|---|---|---|
| POST | `/api/auth/register` | все | регистрация, возвращает токен |
| POST | `/api/auth/login` | все | вход, возвращает токен |
| GET | `/api/auth/me` | с токеном | данные из токена (иначе **401**) |
| GET | `/api/auth/admin` | роль Admin | статистика (иначе **403**) |
| WS | `/chatHub` | с токеном | хаб SignalR |

## Как это работает

**Токен** создаёт `Services/TokenService.cs`. В нём claims `sub`, `unique_name`, `role`, `jti`, срок действия и подпись HMAC-SHA256. Payload можно прочитать без ключа, но подделать нельзя: без секретного ключа не получится правильная подпись.

**Проверка токена** в `Program.cs`:

```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,          // токен перестаёт работать ровно в срок
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
    ...
};
```

**Токен для SignalR.** Браузерный WebSocket не умеет передавать заголовок `Authorization`, поэтому клиент отправляет токен в строке запроса, а сервер забирает его оттуда:

```javascript
// клиент
new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => token })
```

```csharp
// сервер
OnMessageReceived = context =>
{
    var accessToken = context.Request.Query["access_token"];
    if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/chatHub"))
        context.Token = accessToken;
    return Task.CompletedTask;
}
```

**Ограничение доступа к хабу:**

- `[Authorize]` на `ChatHub` и `MapHub<ChatHub>("/chatHub").RequireAuthorization()`;
- метод `ClearChat` — только `[Authorize(Roles = "Admin")]`;
- имя отправителя берётся из проверенного токена, а не от клиента;
- соединение может жить дольше токена, поэтому срок действия проверяется ещё и при каждой отправке сообщения.

## Структура

```
JwtChat/
├── Program.cs                  JWT, SignalR, Swagger
├── appsettings.json            настройки токена
├── Controllers/AuthController.cs   регистрация, вход, /me, /admin
├── Hubs/ChatHub.cs             хаб чата
├── Services/
│   ├── TokenService.cs         генерация JWT
│   └── ConnectionTracker.cs    кто в сети
├── Models/, Dtos/, Data/       пользователи, сообщения, база
└── wwwroot/                    интерфейс (index.html, app.js, signalr.min.js)
```

## Проверка

1. Зарегистрировать двух пользователей в разных браузерах (первый станет Admin) и переписываться.
2. Открыть панель «Токен» и посмотреть payload; токен можно вставить на jwt.io.
3. В Swagger вызвать `GET /api/auth/me` без токена → **401**, с токеном (кнопка Authorize) → **200**.
4. `GET /api/auth/admin` под обычным пользователем → **403**.
5. F12 → Network → WS: подключение к `/chatHub?...&access_token=...`.
6. Поставить `"ExpiresMinutes": 1`, войти и дождаться окончания таймера: клиент выйдет сам.

> Ключ в `appsettings.json` учебный. В реальном проекте ключ хранят в секретах или переменных окружения.
