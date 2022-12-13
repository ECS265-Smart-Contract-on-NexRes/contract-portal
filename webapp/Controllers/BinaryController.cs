using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ContractPortal.Helpers;
using ContractPortal.Models;
using ContractPortal.Models.KVServerInput;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContractPortal.Services;

namespace ContractPortal.Controllers;

[ApiController]
public class BinaryController : OperationController
{
    static readonly Dictionary<Guid, ContractStatus> _dictionary = new Dictionary<Guid, ContractStatus>();
    Process _process;
    string KVSERVER_BASE_PATH = "/home/sssiu/resilientdb";

    private readonly ILogger<BinaryController> _logger;

    public BinaryController(ILogger<BinaryController> logger,
                            Process process,
                            IUserService userService) : base(logger, userService)
    {
        _logger = logger;
        _process = process;
        _logger.LogInformation(KVSERVER_BASE_PATH);
    }

    [Authorize]
    [HttpGet]
    [Route("api/binary/list")]
    public async Task<IEnumerable<ContractStatus>> List()
    {
        var list = new List<ContractStatus>();
        var user = (User)HttpContext.Items["User"];

        foreach (var guid in _dictionary.Keys)
        {
            // query kvserver if the contract previously not published yet
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
                            var contractInputWithSignature = JsonSerializer.Deserialize<InputWithSignature<ContractInput>>(val[1].Trim());
                            _logger.LogInformation($"after deserialization, the signature is: {contractInputWithSignature.Signature}");
                            _logger.LogInformation($"Signature match: {contractInputWithSignature.Signature == _dictionary[guid].Signature}");
                            _dictionary[guid].Content = contractInputWithSignature.Input.ContractContent;
                            _dictionary[guid].IsPublished = true;
                        }
                    }
                }
            }

            // Only shows the contracts uploaded by the conrrent logged-in user
            if (_dictionary[guid].UserId == user.Id)
            {
                list.Add(_dictionary[guid]);
            }
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
        var user = (User)HttpContext.Items["User"];

        if (formFile.Length <= 0)
        {
            return BadRequest(new Exception("File is empty"));
        }

        var resultStr = string.Empty;
        using (var reader = new StreamReader(formFile.OpenReadStream()))
        {
            resultStr = await reader.ReadToEndAsync();

        }
        var contractContent = resultStr.Replace("\"", "\"\"");

        // guid used as contract unique id
        var guid = Guid.NewGuid();

        var input = new ContractInput
        {
            UserId = user.Id,
            ContractId = guid,
            ContractContent = contractContent
        };

        var inputWithSignatureSerialized = CreateInputWithSignature<ContractInput>(input, true);

        var psi = new ProcessStartInfo
        {
            FileName = $"{KVSERVER_BASE_PATH}/bazel-bin/example/kv_server_tools",
            Arguments = $"{KVSERVER_BASE_PATH}/example/kv_client_config.config set {guid} \"{inputWithSignatureSerialized}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        _logger.LogInformation($"Arguments: {$"{KVSERVER_BASE_PATH}/example/kv_client_config.config set {guid} \"{inputWithSignatureSerialized}\""}");

        var proc = new Process
        {
            StartInfo = psi
        };

        proc.Start();
        await proc.WaitForExitAsync();

        _dictionary[guid] = new ContractStatus
        {
            Key = guid,
            Name = name,
            IsPublished = false,
            UserId = user.Id
        };
        return Ok();
    }
}
