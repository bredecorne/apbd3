using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebApplication.DTOs;
using WebApplication.Models;

namespace WebApplication.Controllers;

[ApiController]
[Route("/api/animals")]
public class AnimalsController : ControllerBase
{
    private IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ActionResult<IEnumerable<AnimalDTO>> GetAnimals()
    {
        var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        var sqlCommand = new SqlCommand();
        
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText = "SELECT * FROM Animals";
        
        sqlConnection.Open();

        var sqlData = sqlCommand.ExecuteReader();
        var animals = new List<Animal>();

        while (sqlData.Read())
        {
            animals.Add(new Animal(
                (int) sqlData["id"],
                (string) sqlData["name"],
                (string) sqlData["description"],
                (string) sqlData["category"],
                (string) sqlData["area"])
            );
        }
        
        return Ok();
    }
}