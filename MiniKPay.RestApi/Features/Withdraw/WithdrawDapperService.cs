using Dapper;
using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.User;
using System.Data;

namespace MiniKPay.RestApi.Features.Withdraw;

public class WithdrawDapperService : IWithdrawService
{
	private IUserService _userService;

	public WithdrawDapperService()
	{
		_userService = new UserAdoService();
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

		UserModel user = _userService.GetUser(requestModel.MobileNo);
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

		using IDbConnection connection = new SqlConnection(AppSettings.ConnectionString);
		connection.Open();

		string query = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

		decimal newBalance = user.UserBalance - requestModel.Amount;

		int result = connection.Execute(query, new
		{
			NewBalance = newBalance,
			UserMobileNo = requestModel.MobileNo
		});

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = result > 0 ? "Withdraw successful." : "Withdraw failed.";
		return responseModel;
	}
}
