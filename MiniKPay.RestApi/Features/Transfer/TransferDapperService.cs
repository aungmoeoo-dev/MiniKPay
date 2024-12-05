using Dapper;
using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.User;
using System.Data;

namespace MiniKPay.RestApi.Features.Transfer;

public class TransferDapperService : ITransferService
{
	private IUserService _userService;

	public TransferDapperService()
	{
		_userService = new UserDapperService();
	}

	private bool TransferOperations(
		UserModel fromUser,
		UserModel toUser,
		TransferModel requestModel)
	{
		using IDbConnection connection = new SqlConnection(AppSettings.ConnectionString);

		connection.Open();

		var transaction = connection.BeginTransaction();

		bool isSuccessful;
		try
		{
			string withdrawQuery = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @UserBalance
 WHERE UserMobileNo = @UserMobileNo";

			decimal fromUserNewbalance = fromUser.UserBalance - requestModel.TransactionAmount;

			connection.Execute(withdrawQuery,
				new
				{
					UserBalance = fromUserNewbalance,
					UserMobileNo = fromUser.UserMobileNo
				},transaction);

			string depositQuery = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @UserBalance
 WHERE UserMobileNo = @UserMobileNo";

			decimal toUserNewbalance = toUser.UserBalance + requestModel.TransactionAmount;

			connection.Execute(depositQuery,
				new
				{
					UserBalance = toUserNewbalance,
					UserMobileNo = toUser.UserMobileNo
				}, transaction);

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

			connection.Execute(transactionHistoryQuery,
				new
				{
					FromMobileNo = requestModel.FromMobileNo,
					ToMobileNo = requestModel.ToMobileNo,
					TransactionAmount = requestModel.TransactionAmount,
					TransactionTime = requestModel.TransactionTime,
					TransactionNotes = requestModel.TransactionNotes,
				}, transaction);

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
