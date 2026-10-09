using Microsoft.AspNetCore.Mvc;
using UsersApi.Data;
using UsersApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// отключаем автоматический ответ 400, чтобы невалидные запросы тоже попадали в лог
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSingleton<UpdateLogger>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// создаем базу и таблицы, если их еще нет
try
{
    DbInitializer.Initialize(builder.Configuration.GetConnectionString("UsersDb")!);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Не удалось инициализировать базу данных");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
