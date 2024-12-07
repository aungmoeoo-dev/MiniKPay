using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniKPay.RestApi.Features.TransactionHistory
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionHistoryController : ControllerBase
	{
		private ITransactionHistoryService _transactionHistoryService;

		public TransactionHistoryController()
		{
			_transactionHistoryService = new TransactionHistoryEFCoreService();
		}

		[HttpGet]
		public IActionResult GetTransactions()
		{
			var histories = _transactionHistoryService.GetTransactionHistories();

			return Ok(histories);
		}

		[HttpGet("{mobileNo}")]
		public IActionResult GetTransactionHistory(string mobileNo)
		{
			var history = _transactionHistoryService.GetTransactionHistory(mobileNo);

			if (history is null) return NotFound();

			return Ok(history);
		}
	}
}
