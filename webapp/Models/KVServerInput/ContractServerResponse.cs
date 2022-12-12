using System.Text.Json.Serialization;

namespace ContractPortal.Models.KVServerInput;

public class ContractServerResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("ret")]
    public string Ret { get; set; }
}