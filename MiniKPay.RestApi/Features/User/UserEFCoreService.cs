using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MiniKPay.RestApi.Features.User;

public class UserEFCoreService : IUserService
{
	private AppDbContext _db;

	public UserEFCoreService()
	{
		_db = new AppDbContext();
	}

	public UserModel GetUser(string mobileNo)
	{
		var user = _db.Users.AsNoTracking().FirstOrDefault(user => user.UserMobileNo == mobileNo);

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

		requestModel.UserId = Guid.NewGuid().ToString();
		_db.Users.Add(requestModel);
		int result = _db.SaveChanges();

		responseModel.IsSuccessful = result > 0;
		responseModel.Message = result > 0 ? "Saving successful." : "Saving failed.";
		return responseModel;
	}
}
