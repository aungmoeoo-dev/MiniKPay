using Microsoft.EntityFrameworkCore;
using MiniKPay.RestApi.Features.Transaction;

namespace MiniKPay.RestApi.Features.TransactionHistory;

public class TransactionHistoryEFCoreService : ITransactionHistoryService
{
	private AppDbContext _db;

	public TransactionHistoryEFCoreService()
	{
		_db = new AppDbContext();
	}

	public List<TransactionHistoryModel> GetTransactionHistories()
	{
		var histories = _db.Transactions.AsNoTracking().ToList();

		return histories;
	}
}
