using Microsoft.AspNetCore.Mvc;
using ContractPortal.Models.KVServerInput;
using System.Text.Json;
using ContractPortal.Models;
using ContractPortal.Services;

public class OperationController : ControllerBase
{
    private readonly ILogger<OperationController> _logger;
    private IUserService _userService;

    public OperationController(ILogger<OperationController> logger,
                               IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    protected string CreateInputWithSignature<T>(T input, bool escape) where T : IInput
    {
        var user = (User)HttpContext.Items["User"];
        return CreateInputWithSignature<T>(input, escape, user.Id);
    }

    protected string CreateInputWithSignature<T>(T input, bool escape, string userId) where T : IInput
    {
        var user = _userService.GetAll()
                               .FirstOrDefault(usr => usr.Id == userId);

        // Sign the input with user id and contract unique id
        // and add the signature as part of the input with signature.
        var inputSerilized = JsonSerializer.Serialize(input);
        var signature = Signature.Sign(user.PrivateKey, inputSerilized);
        _logger.LogInformation($"Get balance transaction signature: {signature}");
        var inputWithSignature = new InputWithSignature<T>
        {
            Signature = escape ? signature.Replace("\\", "\\\\") : signature,
            Input = input
        };
        _logger.LogInformation($"Serialized transaction input: {inputSerilized}");
        var inputWithSignatureSerialized = JsonSerializer.Serialize(inputWithSignature);
        if (escape)
        {
            inputWithSignatureSerialized = inputWithSignatureSerialized.Replace("\"", "\"\"");
        }
        return inputWithSignatureSerialized;
    }
}