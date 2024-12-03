using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniKPay.RestApi.Features.User;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
	private UserAdoService _userAdoService;

	public UserController()
	{
		_userAdoService = new UserAdoService();
	}

	[HttpPost]
	public IActionResult CreateUser([FromBody] UserModel requestModel)
	{
		var responseModel = _userAdoService.CreateUser(requestModel);

		if(!responseModel.IsSuccessful) return BadRequest(responseModel);

		return Created("", responseModel);
	}
}
