using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniKPay.RestApi.Features.User;
using System.Data;

namespace MiniKPay.RestApi.Features.Deposit;

public class DepositEFCoreService : IDepositService
{
	private AppDbContext _db;

	public DepositEFCoreService()
	{
		_db = new AppDbContext();
	}

	public DepositResponseModel Deposit(DepositModel requestModel)
	{
		DepositResponseModel responseModel = new();

		if (requestModel.MobileNo is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
			return responseModel;
		}

		if (decimal.IsNegative(requestModel.Amount) || requestModel.Amount == 0)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Invalid Deposit Amount.";
			return responseModel;
		}

		UserModel user = _db.Users.AsNoTracking().FirstOrDefault(user => user.UserMobileNo == requestModel.MobileNo)!;
		if (user is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "User does not exist";
			return responseModel;
		}

		user.UserBalance += requestModel.Amount;
		_db.Entry(user).State = EntityState.Modified;
		int result = _db.SaveChanges();

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = result > 0 ? "Deposit successful." : "Deposit failed.";
		return responseModel;
	}
}
