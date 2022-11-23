using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ContractPortal.Models;

public class Upload
{
    [FromForm(Name="body")]
    public IFormFile Body { get; set; }
    
    [FromForm(Name="name")]
    public string Name { get; set; }
}
