using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.Transaction;
using MiniKPay.RestApi.Features.User;
using System.Data;

namespace MiniKPay.RestApi.Features.TransactionHistory;

public class TransactionHistoryAdoService : ITransactionHistoryService
{
	public List<TransactionHistoryModel> GetTransactionHistories()
	{
		SqlConnection connection = new(AppSettings.ConnectionString);

		connection.Open();

		string query = "select * from TBL_Transaction";
		SqlCommand cmd = new(query, connection);

		SqlDataAdapter adapter = new(cmd);

		DataTable dt = new();
		adapter.Fill(dt);

		connection.Close();

		List<TransactionHistoryModel> histories = new();

		foreach (DataRow row in dt.Rows)
		{
			TransactionHistoryModel model = new()
			{
				TransactionId = row["TransactionId"].ToString(),
				FromMobileNo = row["FromMobileNo"].ToString(),
				ToMobileNo = row["ToMobileNo"].ToString(),
				TransactionAmount = (decimal)row["TransactionAmount"],
				TransactionTime = (DateTime)row["TransactionId"],
				TransactionNotes = row["TransactionNotes"].ToString()
			};

			histories.Add(model);
		}

		return histories;
	}

	public TransactionHistoryModel GetTransactionHistory(string mobileNo)
	{
		throw new NotImplementedException();
	}
}
