namespace wt_lab6_6minimalapi_talko.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Address { get; set; } = "";
    public double Rating { get; set; }
}