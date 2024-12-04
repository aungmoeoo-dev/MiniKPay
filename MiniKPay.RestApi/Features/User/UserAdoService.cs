using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniKPay.RestApi.Features.User;

public class UserAdoService : IUserService
{
	public UserModel GetUser(string mobileNo)
	{
		SqlConnection connection = new(AppSettings.ConnectionString);

		connection.Open();

		string query = "select * from TBL_User where UserMobileNo = @UserMobileNo";
		SqlCommand cmd = new(query, connection);
		cmd.Parameters.AddWithValue("@UserMobileNo", mobileNo);

		SqlDataAdapter adapter = new(cmd);

		DataTable dt = new();
		adapter.Fill(dt);

		connection.Close();

		DataRow row = dt.Rows[0];
		var d = row["UserId"];

		UserModel user = new()
		{
			UserId = row["UserID"].ToString(),
			UserName = row["UserName"].ToString(),
			UserMobileNo = row["UserMobileNo"].ToString(),
			UserPassword = row["UserPassword"].ToString(),
			UserBalance = (decimal)row["UserBalance"]
		};

		return user;
	}

	public UserResponseModel RegisterUser(UserModel requestModel)
	{
		UserResponseModel responseModel = new();

		#region Check if the required info are provided
		if (requestModel.UserName is null
			|| requestModel.UserMobileNo is null
			|| requestModel.UserPassword is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
			return responseModel;
		}
		#endregion

		#region Check if the user already exists
		UserModel user = GetUser(requestModel.UserMobileNo);

		if (user is not null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "User already exists.";
			return responseModel;
		}
		#endregion

		#region Create User
		SqlConnection connection = new(AppSettings.ConnectionString);
		connection.Open();

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

		SqlCommand cmd = new(query, connection);
		cmd.Parameters.AddWithValue("@UserName", requestModel.UserName);
		cmd.Parameters.AddWithValue("@UserMobileNo", requestModel.UserMobileNo);
		cmd.Parameters.AddWithValue("@UserPassword", requestModel.UserPassword);
		cmd.Parameters.AddWithValue("@UserBalance", requestModel.UserBalance);

		int result = cmd.ExecuteNonQuery();

		connection.Close();
		#endregion

		string message = result > 0 ? "Saving successful." : "Saving failed.";

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = message;
		return responseModel;
	}
}
