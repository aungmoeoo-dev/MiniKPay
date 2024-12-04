namespace MiniKPay.RestApi.Features.Withdraw
{
	public interface IWithdrawService
	{
		WithdrawResponseModel Withdraw(WithdrawModel requestModel);
	}
}