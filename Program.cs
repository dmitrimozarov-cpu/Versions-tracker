using Microsoft.EntityFrameworkCore;
using VersionTracker.Api.Data;
using VersionTracker.Api.Collectors;
using VersionTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// База данных PostgreSQL для хранения продуктов и их версий
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// HTTP клиент для сборщиков версий, чтобы они могли делать запросы к внешним API или парсить страницы
builder.Services.AddHttpClient();

// Регистрируем все сборщики версий как сервисы, реализующие IVersionCollector
builder.Services.AddScoped<IVersionCollector, NextCloudCollector>();
builder.Services.AddScoped<IVersionCollector, PlankaCollector>();
builder.Services.AddScoped<IVersionCollector, BigBlueButtonCollector>();
builder.Services.AddScoped<IVersionCollector, CortezaCollector>();
builder.Services.AddScoped<IVersionCollector, NexusCollector>();

// Хранилище для версий, которое будет использоваться контроллерами для получения данных
builder.Services.AddScoped<VersionStorageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Обращаемся к базе данных при запуске приложения, чтобы применить все миграции и создать необходимые таблицы
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();