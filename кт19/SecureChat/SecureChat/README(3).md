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


<img width="1229" height="922" alt="пепа1" src="https://github.com/user-attachments/assets/1ed992d7-1de5-4618-875a-95451c9ea958" />

<img width="1248" height="937" alt="пепа2" src="https://github.com/user-attachments/assets/54a32f7e-9640-4bb1-b811-7fd4ef05db06" />

<img width="1255" height="925" alt="пепа3" src="https://github.com/user-attachments/assets/027b7929-8b50-42dd-9420-2b3a285197df" />

<img width="1232" height="938" alt="пепа4" src="https://github.com/user-attachments/assets/abbbf00d-14d5-42fd-8729-90fa6832617b" />

<img width="1288" height="941" alt="пепа5" src="https://github.com/user-attachments/assets/1d1f2988-ff70-4a83-83d0-257fe050c627" />

<img width="2554" height="905" alt="пепа6" src="https://github.com/user-attachments/assets/f03b7b09-5726-40a8-b296-52ceaf8bc4fc" />
