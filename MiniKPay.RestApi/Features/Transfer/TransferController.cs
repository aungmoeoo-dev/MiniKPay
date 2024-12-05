using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniKPay.RestApi.Features.Transfer;

[Route("api/[controller]")]
[ApiController]
public class TransferController : ControllerBase
{
	private ITransferService _transferAdoService;

	public TransferController()
	{
		_transferAdoService = new TransferEFCoreService();
	}

	[HttpPost]
	public IActionResult Transfer(TransferModel requestModel)
	{
		var responseModel = _transferAdoService.Transfer(requestModel);

		if (!responseModel.IsSuccessful) return BadRequest(responseModel);

		return Ok(responseModel);
	}
}
