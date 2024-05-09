using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebApplication.Models;

namespace WebApplication.Controllers;

[ApiController]
[Route("/api/animals")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Animal>> GetAnimals([FromQuery] string orderBy = "name")
    {
        if (!ValidateOrderByParameter(orderBy)) return BadRequest("Incorrect orderBy parameter.");

        var sqlConnection = new MySqlConnection(_configuration.GetConnectionString("Default"));
        var sqlCommand = new MySqlCommand($"SELECT * FROM Animals ORDER BY {orderBy}", sqlConnection);

        sqlConnection.Open();

        var sqlData = sqlCommand.ExecuteReader();
        var animals = new List<Animal>();

        while (sqlData.Read())
            animals.Add(new Animal(
                (int)sqlData["id"],
                (string)sqlData["name"],
                (string)sqlData["description"],
                (string)sqlData["category"],
                (string)sqlData["area"]
            ));

        return Ok(animals);
    }
    
    [HttpPost] 
    public IActionResult AddAnimal([FromBody] Animal newAnimal)
    {
        var sqlConnection = new MySqlConnection(_configuration.GetConnectionString("Default"));
        using var sqlCommand = new MySqlCommand(
            $"INSERT INTO Animals (Id, Name, Description, Category, Area) VALUES (@id, @name, @description, @category, @area)", 
            sqlConnection);
            
        sqlCommand.Parameters.AddWithValue("@id", newAnimal.Id);
        sqlCommand.Parameters.AddWithValue("@name", newAnimal.Name);
        sqlCommand.Parameters.AddWithValue("@description", newAnimal.Description);
        sqlCommand.Parameters.AddWithValue("@category", newAnimal.Category);
        sqlCommand.Parameters.AddWithValue("@area", newAnimal.Area);
        
        sqlConnection.Open();
        sqlCommand.ExecuteNonQuery();

        return CreatedAtAction(nameof(GetAnimals), new { id = newAnimal.Id }, newAnimal); 
    }

    private bool ValidateOrderByParameter(string orderBy)
    {
        return orderBy.ToLowerInvariant() switch
        {
            "name" or "description" or "category" or "area" => true,
            _ => false
        };
    }
}