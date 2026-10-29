using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wt_lab6_6minimalapi_talko.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Couriers",
                columns: new[] { "Id", "FullName", "Phone", "Vehicle" },
                values: new object[,]
                {
                    { 1, "Иванов Иван", "+7-900-123-45-67", "Автомобиль" },
                    { 2, "Петров Петр", "+7-911-765-43-21", "Велосипед" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Пицца", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Классическая", true, "Пицца Маргарита", 450m },
                    { 2, "Суши", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "С лососем", true, "Суши Филадельфия", 599m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "Address", "CourierId", "DishId", "Status", "TotalPrice" },
                values: new object[,]
                {
                    { 1, "ул. Пушкина, 15", 1, 1, "Доставлен", 450m },
                    { 2, "ул. Гоголя, 5", 2, 2, "В пути", 599m }
                });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "Address", "Rating", "Title" },
                values: new object[,]
                {
                    { 1, "ул. Ленина, 10", 4.7999999999999998, "Итальянский дворик" },
                    { 2, "пр. Мира, 25", 4.5, "Суши Мастер" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Couriers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Couriers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
