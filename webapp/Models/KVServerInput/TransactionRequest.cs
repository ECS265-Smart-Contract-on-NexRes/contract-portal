using System.Text.Json.Serialization;

namespace ContractPortal.Models.KVServerInput;

public class TransactionRequest
{
    [JsonPropertyName("contractId")]
    public Guid ContractId { get; set;}
    [JsonPropertyName("recipient")]
    public string Recipient { get; set; }
    [JsonPropertyName("valStr")]
    public string ValStr { get; set; }
}