using System.Diagnostics;
using System.Text;
using ContractPortal.Helpers;
using ContractPortal.Models;
using Microsoft.AspNetCore.Mvc;
using SockNet.ClientSocket;

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
    SocketClient _client;

    public BalanceController(ILogger<BalanceController> logger,
                            Process process, 
                            SocketClient client)
    {
        _client = client;
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

        var input = $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\"]";
        var privateKey = user.PrivateKey;
        var signature = Signature.Sign(privateKey, input);

        var signedInput = $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\", \"${signature}\"]";

        _logger.LogInformation($"signedInput: {signedInput}");

        string recData = null;
        if (await _client.Connect())
        {
            await _client.Send(Encoding.Default.GetBytes(signedInput));
            byte[] bytes = await _client.ReceiveBytes();
            _client.Disconnect();
            if (bytes != null)
                recData = System.Text.Encoding.UTF8.GetString(bytes);

            _logger.LogInformation($"receive replay from server: {recData}");
        }

        return recData;
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