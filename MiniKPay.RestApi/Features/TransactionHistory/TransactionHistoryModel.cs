using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniKPay.RestApi.Features.Transaction;

[Table("TBL_Transaction")]
public class TransactionHistoryModel
{
	[Key]
	public string? TransactionId { get; set; }
	public string? FromMobileNo { get; set; }
	public string? ToMobileNo { get; set; }
	public decimal TransactionAmount { get; set; }
	public DateTime? TransactionTime { get; set; }
	public string? TransactionNotes { get; set; }
}

public class TransactionHistoryResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}