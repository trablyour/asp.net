using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Services;

var builder = WebApplication.CreateBuilder(args);

// статусы задач в JSON будут строками ("InProgress"), а не числами
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// один обобщенный сервис на все типы ресурсов
builder.Services.AddScoped(typeof(IBookingService<>), typeof(BookingService<>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// создаем базу с начальными данными, если ее нет
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
