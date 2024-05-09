namespace WebApplication.Models;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
    public string Area { get; set; }

    public Animal(int id = default, string name = null, string? description = null, string category = null, string area = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        Area = area;
    }
}