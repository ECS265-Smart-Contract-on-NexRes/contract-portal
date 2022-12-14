namespace ContractPortal.Models;
using System.ComponentModel.DataAnnotations;

public class AuthenticateRequest
{
    public string Username { get; set; }

    [Required]
    public string Id { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public AuthenticationType Type {get;set;}
}

public enum AuthenticationType
{
    Login,
    Registration
}