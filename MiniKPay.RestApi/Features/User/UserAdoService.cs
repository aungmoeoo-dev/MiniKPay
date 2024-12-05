using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniKPay.RestApi.Features.User;

public class UserAdoService : IUserService
{
	public UserModel GetUser(string mobileNo)
	{
		UserModel user = new();
		SqlConnection connection = new(AppSettings.ConnectionString);

		connection.Open();

		string query = "select * from TBL_User where UserMobileNo = @UserMobileNo";
		SqlCommand cmd = new(query, connection);
		cmd.Parameters.AddWithValue("@UserMobileNo", mobileNo);

		SqlDataAdapter adapter = new(cmd);

		DataTable dt = new();
		adapter.Fill(dt);

		connection.Close();

		if (dt.Rows.Count == 0) return user;

		DataRow row = dt.Rows[0];
		user.UserId = row["UserID"].ToString();
		user.UserName = row["UserName"].ToString();
		user.UserMobileNo = row["UserMobileNo"].ToString();
		user.UserPassword = row["UserPassword"].ToString();
		user.UserBalance = (decimal)row["UserBalance"];

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
