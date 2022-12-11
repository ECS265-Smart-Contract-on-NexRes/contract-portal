namespace ContractPortal.Models.KVServerInput;

public class InputWithSignature<T> where T : IInput
{
    public string Signature { get; set; }
    public T Input { get; set; }
}