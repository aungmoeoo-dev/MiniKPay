namespace MiniKPay.RestApi.Features.Transaction;

public class TransactionModel
{
	public string? TransactionId { get; set; }
	public string? FromMobileNo { get; set; }
	public string? ToMobileNo { get; set; }
	public decimal TransactionAmount { get; set; }
	public DateTime? TransactionDate { get; set; }
	public string? TransactionNotes { get; set; }
}

public class TransferRequestModel : TransactionModel
{
	public string? Password	 { get; set; }
}

public class TransactionResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}