namespace MiniKPay.RestApi.Features.Withdraw;

public class WithdrawModel
{
	public string? MobileNo { get; set; }
	public decimal Amount { get; set; }
}

public class WithdrawResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}