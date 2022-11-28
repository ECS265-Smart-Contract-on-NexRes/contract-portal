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
[Route("api/balance/{action}")]
public class BalanceController : ControllerBase
{
    string PYTHON_BASE_PATH;

    private readonly ILogger<BalanceController> _logger;

    public BalanceController(ILogger<BalanceController> logger,
                            Process process)
    {
        _logger = logger;
        PYTHON_BASE_PATH = Environment.GetEnvironmentVariable("PYTHON_BASE_PATH");
        if (PYTHON_BASE_PATH == null) {
            PYTHON_BASE_PATH = "/home/siyuanliu/repos/contract-portal/contractServer";
        }
        _logger.LogInformation(PYTHON_BASE_PATH);
    }
    
    [HttpGet]
    [ActionName("Get")]
    public async Task<string> Get()
    {
        var psi = new ProcessStartInfo
        {
            FileName = $"python3",
            Arguments = $"{PYTHON_BASE_PATH}/getBalance.py",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        _logger.LogInformation($"{PYTHON_BASE_PATH}/getBalance.py");

        var proc = new Process
        {
            StartInfo = psi
        };

        proc.Start();
        await proc.WaitForExitAsync();
        using (System.IO.StreamReader myOutput = proc.StandardOutput)
        {
            var output = myOutput.ReadToEnd();
            _logger.LogInformation($"Get balance: {output}");
            if (output != null &&
                output.Length > 0)
            {
                _logger.LogInformation($"Get balance: {output}");
                return output;
            }
        }
        return null;
    }

    [HttpPost]
    [ActionName("Update")]
    [Route("{add}")]
    public async Task Update(string add)
    {
        var psi = new ProcessStartInfo
        {
            FileName = $"python3",
            Arguments = $"{PYTHON_BASE_PATH}/updateBalance.py {add}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        _logger.LogInformation($"{PYTHON_BASE_PATH}/updateBalance.py {add}");

        var proc = new Process
        {
            StartInfo = psi
        };

        proc.Start();
        await proc.WaitForExitAsync();
    }
}