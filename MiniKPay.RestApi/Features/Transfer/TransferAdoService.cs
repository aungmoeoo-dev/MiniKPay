using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.Transaction;
using MiniKPay.RestApi.Features.User;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniKPay.RestApi.Features.Transfer;

public class TransferAdoService : ITransferService
{
	private IUserService _userService;

	public TransferAdoService()
	{
		_userService = new UserAdoService();
	}

	private bool TransferOperations(
		UserModel fromUser,
		UserModel toUser,
		TransferModel requestModel)
	{
		SqlConnection connection = new(AppSettings.ConnectionString);

		connection.Open();

		SqlTransaction transaction = connection.BeginTransaction();

		SqlCommand cmd = connection.CreateCommand();
		cmd.Transaction = transaction;

		bool isSuccessful;
		try
		{
			string withdrawQuery = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

			decimal fromUserNewbalance = fromUser.UserBalance - requestModel.TransactionAmount;

			cmd.CommandText = withdrawQuery;
			cmd.Parameters.AddWithValue("@NewBalance", fromUserNewbalance);
			cmd.Parameters.AddWithValue("@UserMobileNo", fromUser.UserMobileNo);
			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();

			string depositQuery = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

			decimal toUserNewbalance = toUser.UserBalance + requestModel.TransactionAmount;

			cmd.CommandText = depositQuery;
			cmd.Parameters.AddWithValue("@NewBalance", toUserNewbalance);
			cmd.Parameters.AddWithValue("@UserMobileNo", toUser.UserMobileNo);
			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();

			string transactionHistoryQuery = @"INSERT INTO [dbo].[TBL_Transaction]
           ([FromMobileNo]
           ,[ToMobileNo]
           ,[TransactionAmount]
           ,[TransactionTime]
           ,[TransactionNotes])
     VALUES
           (@FromMobileNo
           ,@ToMobileNo
           ,@TransactionAmount
           ,@TransactionTime
           ,@TransactionNotes)";

			cmd.CommandText = transactionHistoryQuery;
			cmd.Parameters.AddWithValue("@FromMobileNo", requestModel.FromMobileNo);
			cmd.Parameters.AddWithValue("@ToMobileNo", requestModel.ToMobileNo);
			cmd.Parameters.AddWithValue("@TransactionAmount", requestModel.TransactionAmount);
			cmd.Parameters.AddWithValue("@TransactionTime", requestModel.TransactionTime);
			cmd.Parameters.AddWithValue("@TransactionNotes", requestModel.TransactionNotes);
			cmd.ExecuteNonQuery();

			transaction.Commit();

			isSuccessful = true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			try
			{
				transaction.Rollback();
			}
			catch
			{
				Console.WriteLine("Rollback Error!");
			}

			isSuccessful = false;
		}

		connection.Close();

		return isSuccessful;
	}

	public TransferResponseModel Transfer(TransferModel requestModel)
	{
		var responseModel = new TransferResponseModel();

		if (requestModel.FromMobileNo is null
			|| requestModel.ToMobileNo is null
			|| requestModel.TransactionNotes is null
			|| requestModel.Password is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
			return responseModel;
		}

		if (requestModel.TransactionAmount <= 0)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Invalid transfer amount.";
			return responseModel;
		}

		var fromUser = _userService.GetUser(requestModel.FromMobileNo!);
		if (fromUser is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "From User does not exist.";
			return responseModel;
		}

		if (requestModel.Password != fromUser.UserPassword)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Wrong password";
			return responseModel;
		}

		bool hasEnoughBalance = (fromUser.UserBalance - requestModel.TransactionAmount) >= 10000;
		if (!hasEnoughBalance)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Insufficient balance.";
			return responseModel;
		}

		var toUser = _userService.GetUser(requestModel.ToMobileNo!);
		if (toUser is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "To User does not exist.";
			return responseModel;
		}

		requestModel.TransactionTime = DateTime.UtcNow;
		bool isSuccessful = TransferOperations(fromUser, toUser, requestModel);

		responseModel.IsSuccessful = isSuccessful;
		responseModel.Message = isSuccessful ? "Transfer successful." : "Transfer failed.";
		return responseModel;
	}
}
