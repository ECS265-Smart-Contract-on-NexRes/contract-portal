using System.Text.Json.Serialization;

namespace ContractPortal.Models.KVServerInput;

public class ContractInput : IInput
{
    [JsonPropertyName("type")]
    public static string Type {get;} = "contract";
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    [JsonPropertyName("contractId")]
    public Guid ContractId { get; set; }
    [JsonPropertyName("contractContent")]
    public string ContractContent {get;set;}
}