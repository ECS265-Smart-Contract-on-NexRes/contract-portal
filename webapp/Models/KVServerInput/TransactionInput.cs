using System.Text.Json.Serialization;

namespace ContractPortal.Models.KVServerInput;

public class TransactionInput : IInput
{
    [JsonPropertyName("type")]
    public static string Type {get;} = "transaciton";
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    [JsonPropertyName("contractId")]
    public Guid ContractId { get; set; }
    [JsonPropertyName("transactionType")]
    public string  TransactionType { get; set; }
    [JsonPropertyName("params")]
    public List<string> Params {get; set;}
    [JsonPropertyName("transactionId")]
    public Guid TransactionId { get; set; }
}

public static class TransactionInputType {
    public static string Set = "set";
    public static string Get = "get";
    public static string Add = "add";
    public static string Minus = "minus";
}
// $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\", \"${signature}\"]";