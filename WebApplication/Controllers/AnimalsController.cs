using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs;

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
    public ActionResult<IEnumerable<AnimalDTO>> GetAnimals([FromQuery] string orderBy = "name")
    {
        if (!ValidateOrderByParameter(orderBy))
        {
            return BadRequest("Nieprawidłowa wartość parametru orderBy.");
        }

        var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        var sqlCommand = new SqlCommand();

        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText = $"SELECT * FROM Animals ORDER BY {orderBy} ASC";

        sqlConnection.Open();

        var sqlData = sqlCommand.ExecuteReader();
        var animals = new List<AnimalDTO>();

        while (sqlData.Read())
        {
            animals.Add(new AnimalDTO
            {
                Id = (int)sqlData["id"],
                Name = (string)sqlData["name"],
                Description = (string)sqlData["description"],
                Category = (string)sqlData["category"],
                Area = (string)sqlData["area"]
            });
        }

        return Ok(animals);
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