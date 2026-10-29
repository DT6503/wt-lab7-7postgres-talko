namespace wt_lab6_6minimalapi_talko.Models;

public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = "";
    public int Weight { get; set; } // Вес в граммах
}