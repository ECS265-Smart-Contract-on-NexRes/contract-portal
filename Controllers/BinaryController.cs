using ContractPortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractPortal.Controllers;

[ApiController]
[Route("api/binary/{action}")]
public class BinaryController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<BinaryController> _logger;

    public BinaryController(ILogger<BinaryController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ActionName("Get")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost]
    [ActionName("Upload")]
    public async Task<IActionResult> Upload([FromForm]Binary binary)
    {
        var formFile = binary.Body;
        var name = binary.Name;
        if (formFile.Length > 0)
        {
            // full path to file in temp location
            var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
        }
        // process uploaded files
        // Don't rely on or trust the FileName property without validation.
        return Ok();
    }
}
