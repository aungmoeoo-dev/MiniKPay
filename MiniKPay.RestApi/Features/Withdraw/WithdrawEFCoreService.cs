using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniKPay.RestApi.Features.User;
using System.Data;

namespace MiniKPay.RestApi.Features.Withdraw;

public class WithdrawEFCoreService : IWithdrawService
{
	private AppDbContext _db;

	public WithdrawEFCoreService()
	{
		_db = new AppDbContext();
	}

	public WithdrawResponseModel Withdraw(WithdrawModel requestModel)
	{
		WithdrawResponseModel responseModel = new();

		if (requestModel.MobileNo is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
			return responseModel;
		}

		if (decimal.IsNegative(requestModel.Amount) || requestModel.Amount == 0)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Invalid withdraw Amount.";
			return responseModel;
		}

		UserModel user = _db.Users.AsNoTracking().FirstOrDefault(user => user.UserMobileNo == requestModel.MobileNo)!;
		if (user is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "User does not exist";
			return responseModel;
		}

		if (user.UserBalance <= 10000)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Insufficient balance.";
			return responseModel;
		}

		string query = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

		user.UserBalance -= requestModel.Amount;
		_db.Entry(user).State = EntityState.Modified;
		int result = _db.SaveChanges();

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = result > 0 ? "Withdraw successful." : "Withdraw failed.";
		return responseModel;
	}
}
