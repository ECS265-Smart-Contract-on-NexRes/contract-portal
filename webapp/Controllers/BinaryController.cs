using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ContractPortal.Helpers;
using ContractPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContractPortal.Controllers;

[ApiController]
public class BinaryController : ControllerBase
{
    static readonly Dictionary<Guid, KVStatus> _dictionary = new Dictionary<Guid, KVStatus>();
    Process _process;
    string KVSERVER_BASE_PATH = "/home/azureuser/resilientdb";

    private readonly ILogger<BinaryController> _logger;

    public BinaryController(ILogger<BinaryController> logger,
                            Process process)
    {
        _logger = logger;
        _process = process;
        _logger.LogInformation(KVSERVER_BASE_PATH);
    }

    [Authorize]
    [HttpGet]
    [Route("api/binary/list")]
    public async Task<IEnumerable<KVStatus>> List()
    {
        var list = new List<KVStatus>();
        foreach (var guid in _dictionary.Keys)
        {
            if (!_dictionary[guid].IsPublished)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = $"{KVSERVER_BASE_PATH}/bazel-bin/example/kv_server_tools",
                    Arguments = $"{KVSERVER_BASE_PATH}/example/kv_client_config.config get {guid}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                _logger.LogInformation($"{KVSERVER_BASE_PATH}/example/kv_client_config.config get {guid}");

                var proc = new Process
                {
                    StartInfo = psi
                };

                proc.Start();
                await proc.WaitForExitAsync();
                using (System.IO.StreamReader myOutput = proc.StandardOutput)
                {
                    var output = myOutput.ReadToEnd();
                    if (output != null &&
                        output.Length > 00)
                    {
                        var val = output.Split("client get value =");
                        if (val.Length >= 2 &&
                            val[1].Trim().Length > 0)
                        {
                            _dictionary[guid].IsPublished = true;
                            _dictionary[guid].Content = val[1].Trim();
                        }
                    }
                }
            }
            list.Add(_dictionary[guid]);
        }
        return list;
    }

    [Authorize]
    [HttpPost]
    [Route("api/binary/upload")]
    public async Task<IActionResult> Upload([FromForm] Upload binary)
    {
        var formFile = binary.Body;
        var name = binary.Name;

        if (formFile.Length <= 0)
        {
            return BadRequest(new Exception("File is empty"));
        }

        var resultStr = string.Empty;
        using (var reader = new StreamReader(formFile.OpenReadStream()))
        {
            resultStr = await reader.ReadToEndAsync();

        }
        resultStr = resultStr.Replace("\"", "\"\"");
        var guid = Guid.NewGuid();

        var psi = new ProcessStartInfo
        {
            FileName = $"{KVSERVER_BASE_PATH}/bazel-bin/example/kv_server_tools",
            Arguments = $"{KVSERVER_BASE_PATH}/example/kv_client_config.config set {guid} \"{resultStr}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        _logger.LogInformation($"Arguments: {$"{KVSERVER_BASE_PATH}/example/kv_client_config.config set {guid} \"{resultStr}\""}");

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
