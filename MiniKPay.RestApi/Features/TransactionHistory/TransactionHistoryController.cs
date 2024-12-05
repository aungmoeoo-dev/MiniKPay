using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniKPay.RestApi.Features.TransactionHistory;

namespace MiniKPay.RestApi.Features.Transaction
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
	}
}
