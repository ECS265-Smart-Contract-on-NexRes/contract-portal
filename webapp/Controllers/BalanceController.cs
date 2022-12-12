using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ContractPortal.Helpers;
using ContractPortal.Models;
using ContractPortal.Models.KVServerInput;
using Microsoft.AspNetCore.Mvc;
using SockNet.ClientSocket;

namespace ContractPortal.Controllers;

[ApiController]
public class BalanceController : OperationController
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
                            SocketClient client) : base(logger)
    {
        _client = client;
        _logger = logger;
        _logger.LogInformation(PYTHON_BASE_PATH);
    }

    [Authorize]
    [HttpGet]
    [Route("api/balance/get")]
    public async Task<string> Get(Guid contractId)
    {
        var context = HttpContext;
        var user = (User)HttpContext.Items["User"];

        var transactionInput = new TransactionInput
        {
            TransactionType = TransactionInputType.Get,
            UserId = user.Id,
            ContractId = contractId,
            Params = new List<string> { },
            TransactionId = Guid.NewGuid(),
        };

        var inputWithSignatureSerialized = CreateInputWithSignature<TransactionInput>(transactionInput, false);

        //var input = $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\"]";
        //var privateKey = user.PrivateKey;
        //var signature = Signature.Sign(privateKey, input);
        //
        //var signedInput = $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\", \"${signature}\"]";

        _logger.LogInformation($"signedInput: {inputWithSignatureSerialized}");

        ContractServerResponse recData = null;
        if (await _client.Connect())
        {
            await _client.Send(Encoding.Default.GetBytes(inputWithSignatureSerialized));
            byte[] bytes = await _client.ReceiveBytes();
            _client.Disconnect();
            if (bytes != null)
            {
                var recDataStr = System.Text.Encoding.UTF8.GetString(bytes);
                recData = JsonSerializer.Deserialize<ContractServerResponse>(recDataStr);
            }

            _logger.LogInformation($"receive replay from server: {recData}");
        }

        if (!recData.Success) {
            // TODO: ERROR ALERT
        }
        return recData.Ret;
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