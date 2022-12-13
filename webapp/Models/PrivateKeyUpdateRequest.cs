using System.Text.Json.Serialization;

namespace ContractPortal.Models;

public class PrivateKeyUpdateRequest
{
    [JsonPropertyName("newPrivateKey")]
    public string NewPrivateKey { get; set; }
}