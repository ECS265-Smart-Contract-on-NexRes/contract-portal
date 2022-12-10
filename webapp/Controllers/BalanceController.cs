using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ContractPortal.Helpers;
using ContractPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        var signature = Signing(privateKey, input);

        var signedInput = $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\", \"${signature}]\"";

        _logger.LogInformation($"signedInput: {signedInput}");

        string recData = null;
        if (await _client.Connect())
        {
            await _client.Send(Encoding.Default.GetBytes(signedInput));
            byte[] bytes = await _client.ReceiveBytes();
            if (bytes != null)
                recData = bytes.ToString();

            _logger.LogInformation($"receive replay from server: {recData}");
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

    public static string Signing(string key, string msg)
    {
        // Encode the message using UTF-8
        var data = Encoding.UTF8.GetBytes(msg);

        // Import the RSA key
        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(key);

        // Create a PKCS#1 v1.5 signature
        var signer = new RSAPKCS1SignatureFormatter(rsa);
        signer.SetHashAlgorithm("SHA256");

        // Compute the SHA-256 hash of the data
        var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);

        // Sign the hash
        var signature = signer.CreateSignature(hash);

        // Return the signature as a base64-encoded string
        return Convert.ToBase64String(signature);
    }
}