using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.User;

namespace MiniKPay.RestApi.Features.Deposit;

public class DepositAdoService : IDepositService
{
	private IUserService _userService;
	public DepositAdoService()
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

		SqlConnection connection = new(AppSettings.ConnectionString);
		connection.Open();

		string query = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

		decimal newBalance = user.UserBalance + requestModel.Amount;

		SqlCommand cmd = new(query, connection);
		cmd.Parameters.AddWithValue("@NewBalance", newBalance);
		cmd.Parameters.AddWithValue("@UserMobileNo", requestModel.MobileNo);
		int result = cmd.ExecuteNonQuery();

		connection.Close();

		string message = result > 0 ? "Deposit successful." : "Deposit failed.";

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = message;
		return responseModel;
	}
}
