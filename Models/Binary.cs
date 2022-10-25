using Microsoft.AspNetCore.Mvc;

namespace ContractPortal.Models;

public class Binary
{
    [FromForm(Name="body")]
    public IFormFile Body { get; set; }
    
    [FromForm(Name="name")]
    public string Name { get; set; }
}
