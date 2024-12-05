using Dapper;
using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.Transaction;
using System.Data;

namespace MiniKPay.RestApi.Features.TransactionHistory;

public class TransactionHistoryDapperService : ITransactionHistoryService
{
	public List<TransactionHistoryModel> GetTransactionHistories()
	{
		using IDbConnection connection = new SqlConnection(AppSettings.ConnectionString);

		string query = "select * from TBL_Transaction";
		var histories = connection.Query<TransactionHistoryModel>(query).ToList();

		return histories;
	}
}
