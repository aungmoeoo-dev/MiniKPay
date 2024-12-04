using Microsoft.Data.SqlClient;
using MiniKPay.RestApi.Features.Transaction;
using MiniKPay.RestApi.Features.User;

namespace MiniKPay.RestApi.Features.Transfer;

public class TransferAdoService
{
	private IUserService _userService;

	public TransferAdoService()
	{
		_userService = new UserAdoService();
	}

	public TransferResponseModel Transfer(TransferModel requestModel)
	{
		var responseModel = new TransferResponseModel();

		if (requestModel.TransactionId is null
			|| requestModel.FromMobileNo is null
			|| requestModel.ToMobileNo is null
			|| requestModel.TransactionDate is null
			|| requestModel.TransactionNotes is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
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

		if (fromUser.UserBalance <= 10000)
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

		SqlConnection connection = new(AppSettings.ConnectionString);

		connection.Open();

		connection.Close();

		return responseModel;
	}
}
