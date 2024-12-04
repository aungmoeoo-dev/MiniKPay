using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.User;

namespace MiniKPay.RestApi.Features.Withdraw;

public class WithdrawAdoService : IWithdrawService
{
	private IUserService _userService;

	public WithdrawAdoService()
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

		SqlConnection connection = new(AppSettings.ConnectionString);
		connection.Open();

		string query = @"UPDATE [dbo].[TBL_User]
   SET [UserBalance] = @NewBalance
 WHERE UserMobileNo = @UserMobileNo";

		decimal newBalance = user.UserBalance - requestModel.Amount;

		SqlCommand cmd = new(query, connection);
		cmd.Parameters.AddWithValue("@NewBalance", newBalance);
		cmd.Parameters.AddWithValue("@UserMobileNo", requestModel.MobileNo);
		int result = cmd.ExecuteNonQuery();

		connection.Close();

		string message = result > 0 ? "Withdraw successful." : "Withdraw failed.";

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = message;
		return responseModel;
	}
}
