using System.Text.Json.Serialization;

namespace ContractPortal.Models.KVServerInput;

public class InputWithSignature<T> where T : IInput
{
    [JsonPropertyName("signature")]
    public string Signature { get; set; }
    [JsonPropertyName("input")]
    public T Input { get; set; }
}