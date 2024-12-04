namespace MiniKPay.RestApi.Features.Deposit
{
	public interface IDepositService
	{
		DepositResponseModel Deposit(DepositModel requestModel);
	}
}