﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniKPay.RestApi.Features.Deposit;

namespace MiniKPay.RestApi.Features.Withdraw;

[Route("api/[controller]")]
[ApiController]
public class WithdrawController : ControllerBase
{
	private IWithdrawService _withdrawService;

	public WithdrawController()
	{
		_withdrawService = new WithdrawAdoService();
	}

	[HttpPost]
	public IActionResult Withdraw([FromBody] WithdrawModel requestModel)
	{
		var responseModel = _withdrawService.Withdraw(requestModel);

		if (!responseModel.IsSuccessful) return BadRequest(responseModel);

		return Ok(responseModel);
	}
}
