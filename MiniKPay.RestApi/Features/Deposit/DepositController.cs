using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniKPay.RestApi.Features.Deposit;

[Route("api/[controller]")]
[ApiController]
public class DepositController : ControllerBase
{
	private IDepositService _depositAdoService;

	public DepositController()
	{
		_depositAdoService = new DepositAdoService();
	}

	[HttpPost]
	public IActionResult Deposit([FromBody] DepositModel requestModel)
	{
		var responseModel = _depositAdoService.Deposit(requestModel);

		if (!responseModel.IsSuccessful) return BadRequest(responseModel);

		return Ok(responseModel);
	}
}
