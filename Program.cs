using wt_lab6_6minimalapi_talko.Data;
using wt_lab6_6minimalapi_talko.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Принудительно задаем порт для Render
builder.WebHost.UseUrls("http://0.0.0.0:10000");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === НАСТРОЙКА ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

// Если мы на Render (есть переменная DATABASE_URL), парсим её
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    // ИСПРАВЛЕНИЕ: Назвали переменную npgsqlBuilder, чтобы не конфликтовать с builder
    var npgsqlBuilder = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = Uri.UnescapeDataString(userInfo[1]),
        SslMode = Npgsql.SslMode.Require, // Обязательно для Render
        TrustServerCertificate = true
    };

    connectionString = npgsqlBuilder.ConnectionString;
    Console.WriteLine($"Connection string built for host: {uri.Host}");
}

// ИСПРАВЛЕНИЕ: Добавили EnableRetryOnFailure, чтобы приложение не падало, если база "спит"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null)));

var app = builder.Build();

// === БЕЗОПАСНОЕ ПРИМЕНЕНИЕ МИГРАЦИЙ ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Если база не проснулась, приложение продолжит работу, а ошибка будет в логах
        Console.WriteLine($"!!! ОШИБКА МИГРАЦИИ !!!: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Minimal API is working!");
app.MapGet("/api/info", () => new { Application = "Служба доставки еды", Version = "2.0 (PostgreSQL)" });

// ==========================================
// БЛЮДА (DISHES)
// ==========================================

app.MapGet("/api/dishes", async (string? search, AppDbContext db) =>
{
    var query = db.Dishes.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(d => d.Name.ToLower().Contains(search.ToLower()));
    }
    return Results.Ok(await query.ToListAsync());
});

app.MapGet("/api/dishes/{id}", async (int id, AppDbContext db) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
    return dish == null ? Results.NotFound() : Results.Ok(dish);
});

app.MapPost("/api/dishes", async (Dish dish, AppDbContext db) =>
{
    dish.CreatedAt = DateTime.UtcNow;
    db.Dishes.Add(dish);
    await db.SaveChangesAsync();
    return Results.Created($"/api/dishes/{dish.Id}", dish);
});

app.MapPut("/api/dishes/{id}", async (int id, Dish updatedDish, AppDbContext db) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
    if (dish == null) return Results.NotFound();

    dish.Name = updatedDish.Name;
    dish.Category = updatedDish.Category;
    dish.Price = updatedDish.Price;
    dish.IsAvailable = updatedDish.IsAvailable;

    await db.SaveChangesAsync();
    return Results.Ok(dish);
});

app.MapDelete("/api/dishes/{id}", async (int id, AppDbContext db) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
    if (dish == null) return Results.NotFound();

    db.Dishes.Remove(dish);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ==========================================
// КУРЬЕРЫ (COURIERS)
// ==========================================

app.MapGet("/api/couriers", async (AppDbContext db) => Results.Ok(await db.Couriers.ToListAsync()));

app.MapGet("/api/couriers/{id}", async (int id, AppDbContext db) =>
{
    var courier = await db.Couriers.FirstOrDefaultAsync(c => c.Id == id);
    return courier == null ? Results.NotFound() : Results.Ok(courier);
});

app.MapPost("/api/couriers", async (Courier courier, AppDbContext db) =>
{
    db.Couriers.Add(courier);
    await db.SaveChangesAsync();
    return Results.Created($"/api/couriers/{courier.Id}", courier);
});

app.MapPut("/api/couriers/{id}", async (int id, Courier updatedCourier, AppDbContext db) =>
{
    var courier = await db.Couriers.FirstOrDefaultAsync(c => c.Id == id);
    if (courier == null) return Results.NotFound();

    courier.FullName = updatedCourier.FullName;
    courier.Phone = updatedCourier.Phone;
    courier.Vehicle = updatedCourier.Vehicle;

    await db.SaveChangesAsync();
    return Results.Ok(courier);
});

app.MapDelete("/api/couriers/{id}", async (int id, AppDbContext db) =>
{
    var courier = await db.Couriers.FirstOrDefaultAsync(c => c.Id == id);
    if (courier == null) return Results.NotFound();
    db.Couriers.Remove(courier);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ==========================================
// РЕСТОРАНЫ (RESTAURANTS)
// ==========================================

app.MapGet("/api/restaurants", async (AppDbContext db) => Results.Ok(await db.Restaurants.ToListAsync()));

app.MapGet("/api/restaurants/{id}", async (int id, AppDbContext db) =>
{
    var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
    return restaurant == null ? Results.NotFound() : Results.Ok(restaurant);
});

app.MapPost("/api/restaurants", async (Restaurant restaurant, AppDbContext db) =>
{
    db.Restaurants.Add(restaurant);
    await db.SaveChangesAsync();
    return Results.Created($"/api/restaurants/{restaurant.Id}", restaurant);
});

app.MapPut("/api/restaurants/{id}", async (int id, Restaurant updatedRestaurant, AppDbContext db) =>
{
    var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
    if (restaurant == null) return Results.NotFound();

    restaurant.Title = updatedRestaurant.Title;
    restaurant.Address = updatedRestaurant.Address;
    restaurant.Rating = updatedRestaurant.Rating;

    await db.SaveChangesAsync();
    return Results.Ok(restaurant);
});

app.MapDelete("/api/restaurants/{id}", async (int id, AppDbContext db) =>
{
    var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
    if (restaurant == null) return Results.NotFound();
    db.Restaurants.Remove(restaurant);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ==========================================
// ЗАКАЗЫ (ORDERS)
// ==========================================

app.MapGet("/api/orders", async (AppDbContext db) => Results.Ok(await db.Orders.ToListAsync()));

app.MapGet("/api/orders/{id}", async (int id, AppDbContext db) =>
{
    var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
    return order == null ? Results.NotFound() : Results.Ok(order);
});

app.MapPost("/api/orders", async (Order order, AppDbContext db) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == order.DishId);
    if (dish != null) order.TotalPrice = dish.Price;

    db.Orders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/orders/{order.Id}", order);
});

app.MapPut("/api/orders/{id}", async (int id, Order updatedOrder, AppDbContext db) =>
{
    var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
    if (order == null) return Results.NotFound();

    order.DishId = updatedOrder.DishId;
    order.CourierId = updatedOrder.CourierId;
    order.Address = updatedOrder.Address;
    order.Status = updatedOrder.Status;

    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == order.DishId);
    if (dish != null) order.TotalPrice = dish.Price;

    await db.SaveChangesAsync();
    return Results.Ok(order);
});

app.MapDelete("/api/orders/{id}", async (int id, AppDbContext db) =>
{
    var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
    if (order == null) return Results.NotFound();
    db.Orders.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// LINQ 1: Фильтрация по цене
app.MapGet("/api/dishes/expensive", async (AppDbContext db) =>
    Results.Ok(await db.Dishes.Where(d => d.Price > 500).ToListAsync()));

// LINQ 2: Сортировка
app.MapGet("/api/dishes/sorted", async (AppDbContext db) =>
    Results.Ok(await db.Dishes.OrderBy(d => d.Price).ToListAsync()));

// LINQ 3: Выборка с условием
app.MapGet("/api/dishes/available", async (AppDbContext db) =>
    Results.Ok(await db.Dishes.Where(d => d.IsAvailable).OrderBy(d => d.Name).ToListAsync()));

// Свой LINQ-запрос: группировка и подсчёт количества блюд по категориям
app.MapGet("/api/dishes/stats", async (AppDbContext db) =>
{
    var stats = await db.Dishes
        .GroupBy(d => d.Category)
        .Select(g => new
        {
            Category = g.Key,
            Count = g.Count(),
            AveragePrice = g.Average(d => d.Price)
        })
        .ToListAsync();

    return Results.Ok(stats);
});

app.Run(); // В самом конце!