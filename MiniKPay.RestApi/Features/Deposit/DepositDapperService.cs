using Dapper;
using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.User;
using System.Data;

namespace MiniKPay.RestApi.Features.Deposit;

public class DepositDapperService : IDepositService
{
	private IUserService _userService;
	public DepositDapperService()
	{
		_userService = new UserAdoService();
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

		UserModel user = _userService.GetUser(requestModel.MobileNo);
		if (user is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "User does not exist";
			return responseModel;
		}

		using IDbConnection connection = new SqlConnection(AppSettings.ConnectionString);
		connection.Open();

		string query = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

		decimal newBalance = user.UserBalance + requestModel.Amount;

		int result = connection.Execute(query, new
		{
			NewBalance = newBalance,
			UserMobileNo = requestModel.MobileNo
		});

		string message = result > 0 ? "Deposit successful." : "Deposit failed.";

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = message;
		return responseModel;
	}
}
