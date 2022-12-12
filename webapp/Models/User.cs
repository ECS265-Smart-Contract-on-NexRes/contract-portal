using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ContractPortal.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }

    // Never returns password or private key to the brower
    [JsonIgnore]
    public string Password { get; set; }
    [JsonIgnore]
    public string PrivateKey { get; set; }
}
