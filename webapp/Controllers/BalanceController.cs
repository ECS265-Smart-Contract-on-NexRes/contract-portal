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
[Route("api/balance/{action}")]
public class BalanceController : ControllerBase
{
    string PYTHON_BASE_PATH =
#if DEBUG
    "/home/siyuanliu/repos/contract-portal/contractServer";
#else
    "/home/azureuser/contract-portal/contractServer";
#endif
    private readonly ILogger<BalanceController> _logger;

    public BalanceController(ILogger<BalanceController> logger,
                            Process process)
    {
        _logger = logger;
        _logger.LogInformation(PYTHON_BASE_PATH);
    }

    [Authorize]
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
            var output = new string(myOutput.ReadToEnd().Where(c => char.IsDigit(c)).ToArray());
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

    [Authorize]
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