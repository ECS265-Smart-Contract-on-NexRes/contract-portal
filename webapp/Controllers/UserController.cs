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
}