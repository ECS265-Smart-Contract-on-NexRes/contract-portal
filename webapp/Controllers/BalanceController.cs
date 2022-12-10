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
public class BalanceController : ControllerBase
{
    string PYTHON_BASE_PATH =
#if DEBUG
    "/home/siyuanliu/repos/contract-portal/contractServer";
#else
    "/home/sssiu/contract-portal/contractServer";
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
    [Route("api/balance/get")]
    public async Task<string> Get()
    {
        var context = HttpContext;
        var user = (User)HttpContext.Items["User"];

        var psi = new ProcessStartInfo
        {
            FileName = $"python3",
            Arguments = $"{PYTHON_BASE_PATH}/trans_op_meow.py \"{user.PrivateKey}\" get 0 {user.Id}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        _logger.LogInformation(psi.Arguments);

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
    [Route("api/balance/update")]
    public async Task Update(string add)
    {
        var context = HttpContext;
        var user = (User)HttpContext.Items["User"];

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