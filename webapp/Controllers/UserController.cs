namespace ContractPortal.Controllers;

using Microsoft.AspNetCore.Mvc;
using ContractPortal.Helpers;
using ContractPortal.Models;
using ContractPortal.Services;

[ApiController]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("api/user/login")]
    public IActionResult Login(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);

        if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(response);
    }

    [HttpGet]
    [Authorize]
    [Route("api/user/list")]
    public List<User> List()
    {
        return _userService.GetAll()
            .Where(usr => usr.Id != ((User)HttpContext.Items["User"]).Id)
            .ToList();
    }

    [HttpGet]
    [Authorize]
    [Route("api/user/privatekey")]
    public string GetPrivateKey()
    {
        return _userService.GetAll()
            .FirstOrDefault(usr => usr.Id != ((User)HttpContext.Items["User"]).Id)
            .PrivateKey;
    }

    [HttpPut]
    [Authorize]
    [Route("api/user/privatekey/{key}")]
    public void PutPrivateKey(string key)
    {
        var user =_userService.GetAll()
            .FirstOrDefault(usr => usr.Id != ((User)HttpContext.Items["User"]).Id);
        user.PrivateKey = key;
    }
}