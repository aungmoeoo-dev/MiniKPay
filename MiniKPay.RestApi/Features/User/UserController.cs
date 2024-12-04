using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniKPay.RestApi.Features.User;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
	private IUserService _userService;

	public UserController()
	{
		_userService = new UserDapperService();
	}

	[HttpPost]
	public IActionResult RegisterUser([FromBody] UserModel requestModel)
	{
		var responseModel = _userService.RegisterUser(requestModel);

		if (!responseModel.IsSuccessful) return BadRequest(responseModel);

		return Created("", responseModel);
	}

	[HttpGet("{mobileNo}")]
	public IActionResult GetUser([FromBody] string mobileId)
	{
		var user = _userService.GetUser(mobileId);

		if (user is null) return NotFound(user);

		return Ok(user);
	}
}
