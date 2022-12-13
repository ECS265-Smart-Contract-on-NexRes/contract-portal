using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ContractPortal.Helpers;
using ContractPortal.Models;
using ContractPortal.Models.KVServerInput;
using Microsoft.AspNetCore.Mvc;
using SockNet.ClientSocket;
using ContractPortal.Services;

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
                            SocketClient client,
                            IUserService userService) : base(logger, userService)
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

        var transactionInput = new OperationInput
        {
            Type = OperationInputType.Get,
            UserId = user.Id,
            ContractId = contractId,
            Params = new List<object> { },
            TransactionId = Guid.NewGuid(),
        };

        var inputWithSignatureSerialized = CreateInputWithSignature<OperationInput>(transactionInput, false);

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
        var user = (User)HttpContext.Items["User"];
        var senderId = user.Id;

        var recipient = request.Recipient;
        var contractId = request.ContractId;
        var valStr = request.ValStr;

        var val = int.Parse(valStr);

        var transacInput = new TransactionInput
        {
            Sender = senderId,
            AddRecipientBalanceOp = new OperationInput
            {
                Type = OperationInputType.Add,
                UserId = recipient,
                ContractId = Guid.Parse(contractId),
                Params = new List<object> { val },
                TransactionId = Guid.NewGuid()
            },
            MinusSenderBalanceOp = new OperationInput
            {
                Type = OperationInputType.Minus,
                UserId = senderId,
                ContractId = Guid.Parse(contractId),
                Params = new List<object> { val },
                TransactionId = Guid.NewGuid()
            }
        };

        var inputWithSignatureSerialized = CreateInputWithSignature<TransactionInput>(transacInput, false, senderId);

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
                User Id:     {senderId}
                type:        transaction
                value:       {val}
                receive replay from server regarding  : {recDataStr}");
        }

        if (!recData.Success)
        {
            throw new Exception(recData.Ret);
        }
    }
}