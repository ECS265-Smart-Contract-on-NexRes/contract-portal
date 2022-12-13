namespace ContractPortal.Controllers;

using Microsoft.AspNetCore.Mvc;
using ContractPortal.Helpers;
using ContractPortal.Models;
using ContractPortal.Services;

[ApiController]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
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
    public object GetPrivateKey()
    {
        var privateKey = _userService.GetAll()
            .FirstOrDefault(usr => usr.Id == ((User)HttpContext.Items["User"]).Id)
            .PrivateKey;
        return new { privateKey = privateKey };
    }

    [HttpPut]
    [Authorize]
    [Route("api/user/privatekey")]
    public void PutPrivateKey([FromBody] PrivateKeyUpdateRequest request)
    {
        var user = _userService.GetAll()
            .FirstOrDefault(usr => usr.Id == ((User)HttpContext.Items["User"]).Id);
        _logger.LogInformation($" New Private Key for {user.Id}: {request.NewPrivateKey}");
        user.PrivateKey = request.NewPrivateKey;
    }
}