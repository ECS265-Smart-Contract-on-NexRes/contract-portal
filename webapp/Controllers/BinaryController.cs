using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ContractPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContractPortal.Controllers;

[ApiController]
[Route("api/binary/{action}")]
public class BinaryController : ControllerBase
{
    static readonly Dictionary<Guid, KVStatus> _dictionary = new Dictionary<Guid, KVStatus>();
    Process _process;
    string KVSERVER_BASE_PATH = Environment.GetEnvironmentVariable("KVSERVER_BASE_PATH");

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<BinaryController> _logger;

    public BinaryController(ILogger<BinaryController> logger,
                            Process process)
    {
        _logger = logger;
        _process = process;
    }

    [HttpGet]
    [ActionName("List")]
    public IEnumerable<KVStatus> List()
    {
        var list = new List<KVStatus>();
        foreach (var guid in _dictionary.Keys)
        {
            list.Add(_dictionary[guid]);
        }
        return list;
    }

    [HttpPost]
    [ActionName("Upload")]
    public async Task<IActionResult> Upload([FromForm] Upload binary)
    {
        var formFile = binary.Body;
        var name = binary.Name;

        if (formFile.Length <= 0)
        {
            return BadRequest(new Exception("File is empty"));
        }

        var result = new StringBuilder();
        using (var reader = new StreamReader(formFile.OpenReadStream()))
        {
            while (reader.Peek() >= 0)
            {
                result.AppendLine(await reader.ReadLineAsync());
            }
        }
        var resultStr = result.ToString();
        var guid = Guid.NewGuid();

        var psi = new ProcessStartInfo
        {
            FileName = $"{KVSERVER_BASE_PATH}/bazel-bin/example/kv_server_tools",
            Arguments = $"{KVSERVER_BASE_PATH}/example/kv_client_config.config set {guid} {resultStr}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var proc = new Process
        {
            StartInfo = psi
        };

        proc.Start();
        await proc.WaitForExitAsync();


        _dictionary[guid] = new KVStatus
        {
            Key = guid,
            Name = name,
            IsPublished = false
        };
        return Ok();
    }
}
