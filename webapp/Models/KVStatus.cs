using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace ContractPortal.Models;

public class KVStatus
{
    public Guid Key { get; set; }
    public bool IsPublished { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
}
