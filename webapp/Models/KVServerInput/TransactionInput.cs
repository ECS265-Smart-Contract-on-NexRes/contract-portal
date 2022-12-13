using System.Text.Json.Serialization;

namespace ContractPortal.Models.KVServerInput;

public class OperationInput : IInput
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    [JsonPropertyName("contractId")]
    public Guid ContractId { get; set; }
    [JsonPropertyName("params")]
    public List<object> Params { get; set; }
    [JsonPropertyName("transactionId")]
    public Guid TransactionId { get; set; }
}

public class TransactionInput : IInput
{
    [JsonPropertyName("type")]
    public string Type { get { return OperationInputType.Transaction;} }
    [JsonPropertyName("addRecipientBalanceOp")]
    public OperationInput AddRecipientBalanceOp { get; set; }
    [JsonPropertyName("minusSenderBalanceOp")]
    public OperationInput MinusSenderBalanceOp { get; set; }
    [JsonPropertyName("sender")]
    public string Sender { get; set; }
}

public static class OperationInputType
{
    public static string Contract = "contract";
    public static string Set = "set";
    public static string Get = "get";
    public static string Add = "add";
    public static string Minus = "minus";
    public static string Transaction = "transaction";
}
// $"[\"{user.Id}\", \"test.sol\", \"get\", [], \"1001\", \"${signature}\"]";