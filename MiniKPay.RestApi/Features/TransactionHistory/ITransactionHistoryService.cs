using MiniKPay.RestApi.Features.Transaction;

namespace MiniKPay.RestApi.Features.TransactionHistory
{
	public interface ITransactionHistoryService
	{
		List<TransactionHistoryModel> GetTransactionHistories();
	}
}