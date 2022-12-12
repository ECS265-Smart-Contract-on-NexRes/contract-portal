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

        if (!recData.Success)
        {
            throw new Exception(recData.Ret);
        }
        return recData.Ret;
    }

    [Authorize]
    [HttpPost]
    [Route("api/balance/update")]
    public async Task Update([FromBody] TransactionRequest request)
    {
        var recipient = request.Recipient;
        var contractId = request.ContractId;
        var valStr = request.ValStr;

        var val = int.Parse(valStr);
        var userBalanceStr = await Get(contractId);
        int userBalance;
        if (!int.TryParse(userBalanceStr, out userBalance) ||
            userBalance - val < 0) {
            throw new Exception("User does not have enough balance to make the transaciton!");
        }

        var context = HttpContext;
        var user = (User)HttpContext.Items["User"];
        await Transfer(contractId, recipient, TransactionInputType.Add, valStr);
        await Transfer(contractId, recipient, TransactionInputType.Minus, valStr);
    }

    private async Task Transfer(Guid contractId, string userId, string method, string val)
    {
        var transactionInput = new TransactionInput
        {
            TransactionType = method,
            UserId = userId,
            ContractId = contractId,
            Params = new List<string> { val },
            TransactionId = Guid.NewGuid(),
        };

        var inputWithSignatureSerialized = CreateInputWithSignature<TransactionInput>(transactionInput, false);

        _logger.LogInformation($"signedInput: {inputWithSignatureSerialized}");

        ContractServerResponse recData = null;
        string recDataStr = string.Empty;
        if (await _client.Connect())
        {
            await _client.Send(Encoding.Default.GetBytes(inputWithSignatureSerialized));
            byte[] bytes = await _client.ReceiveBytes();
            _client.Disconnect();
            if (bytes != null)
            {
                recDataStr = System.Text.Encoding.UTF8.GetString(bytes);
                recData = JsonSerializer.Deserialize<ContractServerResponse>(recDataStr);
            }

            _logger.LogInformation(@$"
                Contract Id: {contractId}
                User Id:     {userId}
                method:      {method}
                value:       {val}
                receive replay from server regarding  : {recDataStr}");
        }

        if (!recData.Success)
        {
            throw new Exception(recData.Ret);
        }
    }
}