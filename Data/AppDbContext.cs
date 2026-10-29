using Microsoft.EntityFrameworkCore;
using wt_lab6_6minimalapi_talko.Models;

namespace wt_lab6_6minimalapi_talko.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<Courier> Couriers => Set<Courier>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. НАЧАЛЬНЫЕ ДАННЫЕ ДЛЯ РЕСТОРАНОВ
        modelBuilder.Entity<Restaurant>().HasData(
            new Restaurant
            {
                Id = 1,
                Title = "Итальянский дворик",
                Address = "ул. Ленина, 10",
                Rating = 4.8
            },
            new Restaurant
            {
                Id = 2,
                Title = "Суши Мастер",
                Address = "пр. Мира, 25",
                Rating = 4.5
            }
        );

        // 2. НАЧАЛЬНЫЕ ДАННЫЕ ДЛЯ КУРЬЕРОВ
        modelBuilder.Entity<Courier>().HasData(
            new Courier
            {
                Id = 1,
                FullName = "Иванов Иван",
                Phone = "+7-900-123-45-67",
                Vehicle = "Автомобиль"
            },
            new Courier
            {
                Id = 2,
                FullName = "Петров Петр",
                Phone = "+7-911-765-43-21",
                Vehicle = "Велосипед"
            }
        );

        // 3. НАЧАЛЬНЫЕ ДАННЫЕ ДЛЯ БЛЮД (Обратите внимание на DateTimeKind.Utc)
        modelBuilder.Entity<Dish>().HasData(
            new Dish
            {
                Id = 1,
                Name = "Пицца Маргарита",
                Category = "Пицца",
                Price = 450m,
                IsAvailable = true,
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026, 1, 1), DateTimeKind.Utc),
                Description = "Классическая"
            },
            new Dish
            {
                Id = 2,
                Name = "Суши Филадельфия",
                Category = "Суши",
                Price = 599m,
                IsAvailable = true,
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026, 1, 2), DateTimeKind.Utc),
                Description = "С лососем"
            }
        );

        // 4. НАЧАЛЬНЫЕ ДАННЫЕ ДЛЯ ЗАКАЗОВ
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                DishId = 1,
                CourierId = 1,
                Address = "ул. Пушкина, 15",
                TotalPrice = 450m,
                Status = "Доставлен"
            },
            new Order
            {
                Id = 2,
                DishId = 2,
                CourierId = 2,
                Address = "ул. Гоголя, 5",
                TotalPrice = 599m,
                Status = "В пути"
            }
        );
    }
}