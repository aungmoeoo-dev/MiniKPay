using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniKPay.RestApi.Features.User;

public class UserAdoService
{

	public bool IsMobileNoExist(string mobileNo)
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

		return dt.Rows.Count > 0;
	}

	public UserResponseModel CreateUser(UserModel requestModel)
	{
		UserResponseModel responseModel = new();

		#region Check if the required info are provided
		if (requestModel.Name is null
			|| requestModel.MobileNo is null
			|| requestModel.Password is null)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Required info not provided.";
			return responseModel;
		}
		#endregion

		#region Check if the mobile number already exists
		bool isExist = IsMobileNoExist(requestModel.MobileNo);

		if (isExist)
		{
			responseModel.IsSuccessful = false;
			responseModel.Message = "Mobile No. already exists.";
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
		cmd.Parameters.AddWithValue("@UserName", requestModel.Name);
		cmd.Parameters.AddWithValue("@UserMobileNo", requestModel.MobileNo);
		cmd.Parameters.AddWithValue("@UserPassword", requestModel.Password);
		cmd.Parameters.AddWithValue("@UserBalance", requestModel.Balance);

		int result = cmd.ExecuteNonQuery();

		connection.Close();
		#endregion

		string message = result > 0 ? "Saving successful." : "Saving failed.";

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = message;
		return responseModel;
	}
}
