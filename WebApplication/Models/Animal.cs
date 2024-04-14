namespace WebApplication.Models;

public class Animal
{
    private int Id { get; set; }
    private string Name { get; set; }
    private string? Description { get; set; }
    private string Category { get; set; }
    private string Area { get; set; }

    public Animal(int id = default, string name = null, string? description = null, string category = null, string area = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        Area = area;
    }
}