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
    
    [HttpPut("{id}")]
    public IActionResult UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
    {
        var sqlConnection = new MySqlConnection(_configuration.GetConnectionString("Default"));
        using var sqlCommand = new MySqlCommand($"SELECT * FROM Animals WHERE Id = @id", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@id", id);

        sqlConnection.Open();
        var reader = sqlCommand.ExecuteReader();
        if (!reader.HasRows)
        {
            return NotFound();
        }
        reader.Close();

        using var updateCommand = new MySqlCommand(
            $"UPDATE Animals SET Name = @name, Description = @description, Category = @category, Area = @area WHERE Id = @id", 
            sqlConnection);

        updateCommand.Parameters.AddWithValue("@id", id);
        updateCommand.Parameters.AddWithValue("@name", updatedAnimal.Name);
        updateCommand.Parameters.AddWithValue("@description", updatedAnimal.Description);
        updateCommand.Parameters.AddWithValue("@category", updatedAnimal.Category);
        updateCommand.Parameters.AddWithValue("@area", updatedAnimal.Area);

        updateCommand.ExecuteNonQuery();

        return NoContent(); 
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteAnimal(int id)
    {
        var sqlConnection = new MySqlConnection(_configuration.GetConnectionString("Default"));
        using var sqlCommand = new MySqlCommand($"DELETE FROM Animals WHERE Id = @id", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@id", id);

        sqlConnection.Open();
        var rowsAffected = sqlCommand.ExecuteNonQuery();

        if (rowsAffected == 0)
        { 
            return NotFound();
        }

        return NoContent();
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