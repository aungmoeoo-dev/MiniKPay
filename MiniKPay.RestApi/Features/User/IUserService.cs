namespace MiniKPay.RestApi.Features.User
{
	public interface IUserService
	{
		UserResponseModel RegisterUser(UserModel requestModel);
		UserModel GetUser(string mobileNo);
	}
}