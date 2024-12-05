using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniKPay.RestApi.Features.Transaction;
using MiniKPay.RestApi.Features.User;

namespace MiniKPay.RestApi.Features.Transfer;

public class TransferEFCoreService : ITransferService
{
	private IUserService _userService;
	private AppDbContext _db;

	public TransferEFCoreService()
	{
		_userService = new UserEFCoreService();
		_db = new AppDbContext();
	}

	private bool TransferOperations(
		UserModel fromUser,
		UserModel toUser,
		TransferModel requestModel)
	{

		using var transaction = _db.Database.BeginTransaction();

		bool isSuccessful;
		try
		{
			fromUser = _db.Users.AsNoTracking().FirstOrDefault(user => user.UserMobileNo == fromUser.UserMobileNo)!;
			decimal fromUserNewbalance = fromUser.UserBalance - requestModel.TransactionAmount;
			fromUser.UserBalance = fromUserNewbalance;
			_db.Entry(fromUser).State = EntityState.Modified;
			_db.SaveChanges();

			toUser = _db.Users.AsNoTracking().FirstOrDefault(user => user.UserMobileNo == toUser.UserMobileNo)!;
			decimal toUserNewbalance = toUser.UserBalance + requestModel.TransactionAmount;
			toUser.UserBalance = toUserNewbalance;
			_db.Entry(toUser).State = EntityState.Modified;
			_db.SaveChanges();

			TransactionHistoryModel transactionHistory = new()
			{
				TransactionId = Guid.NewGuid().ToString(),
				FromMobileNo = requestModel.FromMobileNo,
				ToMobileNo = requestModel.ToMobileNo,
				TransactionAmount = requestModel.TransactionAmount,
				TransactionTime = requestModel.TransactionTime,
				TransactionNotes = requestModel.TransactionNotes,
			};

			_db.Transactions.Add(transactionHistory);
			_db.SaveChanges();

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
