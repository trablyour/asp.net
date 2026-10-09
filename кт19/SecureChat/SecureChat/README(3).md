# SecureChat — защищённый чат на SignalR с куки-аутентификацией

Веб-приложение на **ASP.NET Core MVC** и **SignalR**: регистрация и вход на основе **куки**, чат в реальном времени, доступный только авторизованным пользователям.

## Возможности

- регистрация и вход, флажок «Запомнить меня», выход;
- пароли хранятся только в виде хэша;
- чат в реальном времени: история последних 50 сообщений, список пользователей в сети, уведомления «вошёл / вышел», индикатор «печатает...», статус соединения и автоматическое переподключение;
- свои сообщения отображаются справа;
- без входа страница чата перенаправляет на страницу входа, а хаб отказывает в подключении.

## Технологии

.NET 7 / 8 · ASP.NET Core MVC · Cookie Authentication · SignalR · Entity Framework Core · SQLite · JavaScript

## Запуск

```bash
dotnet run
```

Открыть http://localhost:5000. База `chat.db` создаётся автоматически. Клиентская библиотека SignalR лежит в проекте (`wwwroot/lib/signalr`), интернет не нужен.

## Как это работает

**Куки-аутентификация** настроена в `Program.cs` (без Identity):

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "SecureChat.Auth";
        options.Cookie.HttpOnly = true;            // кука недоступна из JavaScript
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;          // продлевается при активности
    });
```

Порядок middleware важен:

```csharp
app.UseAuthentication();   // кто пользователь
app.UseAuthorization();    // что ему можно
```

**Вход** (`AccountController`): проверяется хэш пароля (`PasswordHasher`), создаётся `ClaimsPrincipal` с claims Id и Name, вызывается `HttpContext.SignInAsync`. Сервер выдаёт куку, и браузер дальше сам отправляет её с каждым запросом, в том числе при подключении к хабу. «Запомнить меня» делает куку постоянной на 7 дней. На неверный логин и неверный пароль выдаётся одна и та же ошибка, чтобы нельзя было узнать, существует ли логин.

**Ограничение доступа к чату** в трёх местах:

| Где | Что делает |
|---|---|
| `[Authorize]` на `ChatController` | без входа страница чата перенаправляет на `/Account/Login`, после входа возвращает обратно |
| `[Authorize]` на `ChatHub` + `MapHub<ChatHub>("/chatHub").RequireAuthorization()` | без куки подключиться к хабу нельзя (сервер отвечает 401) |
| `Context.User.Identity.Name` в хабе | имя отправителя берётся из куки, а не от клиента, поэтому его нельзя подделать |

**Безопасность сообщений:** текст вставляется на страницу через `textContent`, поэтому HTML в сообщении не выполнится (защита от XSS); длина сообщения ограничена 500 символами и проверяется на сервере.

## Методы хаба

| Метод / событие | Направление | Назначение |
|---|---|---|
| `SendMessage(text)` | клиент → сервер | отправить сообщение |
| `GetHistory()` | клиент → сервер | последние 50 сообщений |
| `Typing()` | клиент → сервер | «печатает...» |
| `ReceiveMessage` | сервер → клиенты | новое сообщение |
| `OnlineUsers` | сервер → клиенты | список пользователей в сети |
| `UserJoined` / `UserLeft` | сервер → клиенты | вход и выход из чата |
| `UserTyping` | сервер → клиенты | кто печатает |

## Структура

```
SecureChat/
├── Program.cs                     куки, SignalR, middleware
├── Controllers/
│   ├── HomeController.cs
│   ├── AccountController.cs       регистрация, вход, выход
│   └── ChatController.cs          [Authorize] страница чата
├── Hubs/ChatHub.cs                [Authorize] хаб чата
├── Services/ConnectionTracker.cs  кто в сети (с учётом нескольких вкладок)
├── Models/                        User, ChatMessage
├── ViewModels/                    формы входа и регистрации
├── Data/AppDbContext.cs
├── Views/                         страницы
└── wwwroot/
    ├── js/chat.js                 клиент SignalR
    └── lib/signalr/signalr.min.js
```

## Проверка

1. Без входа открыть `/Chat`: перенаправление на страницу входа с сообщением «нужно войти».
2. Зарегистрироваться, перейти в чат.
3. Открыть второй браузер (или окно инкогнито), зарегистрировать второго пользователя: сообщения приходят друг другу мгновенно, работают «в сети» и «печатает...».
4. F12 → Application → Cookies: кука `SecureChat.Auth` с флагом HttpOnly.
5. Выйти: чат снова недоступен.
