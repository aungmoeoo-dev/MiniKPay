using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniKPay.RestApi.Features.User;

public class UserDapperService : IUserService
{
	public UserModel GetUser(string mobileNo)
	{
		using IDbConnection connection = new SqlConnection(AppSettings.ConnectionString);

		string query = "select * from TBL_User where UserMobileNo = @UserMobileNo";
		var user = connection
			.QueryFirstOrDefault<UserModel>(query, new { UserMobileNo = mobileNo });

		return user!;
	}

	public UserResponseModel RegisterUser(UserModel requestModel)
	{
		UserResponseModel responseModel = new();

		if (requestModel.UserName is null
			|| requestModel.UserMobileNo is null
			|| requestModel.UserPassword is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
			return responseModel;
		}

		UserModel user = GetUser(requestModel.UserMobileNo);

		if (user is not null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "User already exists.";
			return responseModel;
		}

		using IDbConnection connection = new SqlConnection(AppSettings.ConnectionString);

		string query = @"INSERT INTO [dbo].[TBL_User]
           ([UserName]
           ,[UserMobileNo]
           ,[UserPassword]
           ,[UserBalance])
     VALUES
           (@UserName
           ,@UserMobileNo
           ,@UserPassword
           ,@UserBalance)";

		int result = connection.Execute(query, new
		{
			UserName = requestModel.UserName,
			UserMobileNo = requestModel.UserMobileNo,
			UserPassword = requestModel.UserPassword,
			UserBalance = requestModel.UserBalance
		});

		string message = result > 0 ? "Saving successful." : "Saving failed.";

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = message;
		return responseModel;
	}
}
