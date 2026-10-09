# AdminPanel — Identity, роли и админ-панель

Веб-приложение на **ASP.NET Core MVC** с аутентификацией через **ASP.NET Core Identity**, ролями, политиками авторизации и админ-панелью для управления пользователями.

## Возможности

- регистрация, вход и выход пользователей;
- две роли: **Admin** и **User** (все новые пользователи получают роль User);
- политики авторизации `AdminOnly` и `RegisteredUser`;
- админ-панель с навигацией по разделам **Пользователи**, **Настройки**, **Отчёты**;
- CRUD пользователей: просмотр, добавление, редактирование (имя, email, роль, пароль), удаление с подтверждением;
- настройки сайта: название и включение / отключение регистрации;
- отчёты: количество пользователей по ролям, новые за 7 дней, последние регистрации;
- защита от ошибок администратора: нельзя удалить себя и снять с себя роль Admin.

## Технологии

.NET 7 / 8 · ASP.NET Core MVC · ASP.NET Core Identity · Entity Framework Core · SQLite

## Запуск

```bash
dotnet run
```

Открыть http://localhost:5000. База `adminpanel.db`, роли и тестовые аккаунты создаются автоматически при первом запуске.

| Роль | Email | Пароль |
|---|---|---|
| Администратор | `admin@site.ru` | `Admin123` |
| Пользователь | `user@site.ru` | `User123` |

## Структура

```
AdminPanel/
├── Program.cs                         Identity, политики, маршруты
├── Models/
│   ├── ApplicationUser.cs             пользователь + FullName, CreatedAt
│   └── Roles.cs                       имена ролей и политик
├── Data/
│   ├── AppDbContext.cs                IdentityDbContext
│   └── DbInitializer.cs               создание ролей и тестовых аккаунтов
├── Services/
│   ├── SiteSettings.cs                настройки сайта
│   └── RussianIdentityErrorDescriber.cs   ошибки Identity на русском
├── Controllers/
│   ├── HomeController.cs              главная, профиль
│   └── AccountController.cs           вход, регистрация, выход
└── Areas/Admin/
    ├── Controllers/
    │   ├── AdminControllerBase.cs     [Area("Admin")] + [Authorize(Policy = "AdminOnly")]
    │   ├── DashboardController.cs
    │   ├── UsersController.cs         CRUD пользователей
    │   ├── SettingsController.cs
    │   └── ReportsController.cs
    └── Views/                         шаблон админ-панели и формы
```

## Как это работает

**Identity** подключается в `Program.cs`:

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { ... })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

- `UserManager` — создание, поиск, роли, смена пароля;
- `SignInManager` — вход и выход;
- `RoleManager` — роли.

**Политики авторизации**:

```csharp
options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
options.AddPolicy("RegisteredUser", policy => policy.RequireRole("Admin", "User"));
```

Вся админ-панель закрыта одним атрибутом на базовом классе `AdminControllerBase`, от которого наследуются все её контроллеры. Пользователь без роли Admin при попытке открыть `/Admin` попадает на страницу «Доступ запрещён».

**Пароли** хранятся только в виде хэша. Смена пароля из админки выполняется через токен сброса: `GeneratePasswordResetTokenAsync` → `ResetPasswordAsync`.

## Проверка

1. Войти как `user@site.ru`: ссылки «Админ-панель» нет, адрес `/Admin` даёт «Доступ запрещён».
2. Войти как `admin@site.ru`: открыть админ-панель, пройти разделы.
3. Создать пользователя, изменить ему роль и пароль, удалить.
4. В настройках выключить регистрацию → страница регистрации закрывается.
