using ContractPortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractPortal.Controllers;

[ApiController]
[Route("api/binary/{action}")]
public class BinaryController : ControllerBase
{

    const string _binDir = "~/binaries";

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<BinaryController> _logger;

    public BinaryController(ILogger<BinaryController> logger)
    {
        _logger = logger;
        if (!Directory.Exists(_binDir))
        {
            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(_binDir);
        }
    }

    [HttpGet]
    [ActionName("List")]
    public IEnumerable<string> List()
    {
        DirectoryInfo d = new DirectoryInfo(_binDir); //Assuming Test is your Folder

        FileInfo[] Files = d.GetFiles(); //Getting Text files
        var files = new List<string>();

        foreach (FileInfo file in Files)
        {
            files.Add(file.Name);
        }
        return files;
    }

    [HttpGet]
    [ActionName("Download")]
    public IActionResult Download(string name)
    {
        Stream stream = new FileStream(Path.Combine(_binDir, name), FileMode.Open);

        if (stream == null)
            return NotFound(); // returns a NotFoundResult with Status404NotFound response.

        return File(stream, "application/octet-stream"); // returns a FileStreamResult
    }

    [HttpPost]
    [ActionName("Upload")]
    public async Task<IActionResult> Upload([FromForm] Binary binary)
    {
        var formFile = binary.Body;
        var name = binary.Name;
        if (formFile.Length > 0)
        {
            // full path to file in temp location
            var filePath = Path.Combine(_binDir, name); //we are using Temp file name just for the example. Add your own file path.
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
