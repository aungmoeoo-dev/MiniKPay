namespace MiniKPay.RestApi.Features.Deposit;

public class DepositModel
{
	public string? MobileNo { get; set; }
	public decimal Amount { get; set; }
}

public class DepositResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}