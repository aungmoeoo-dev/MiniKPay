﻿using MiniKPay.RestApi.Features.Transaction;

namespace MiniKPay.RestApi.Features.Transfer
{
	public class TransferModel: TransactionHistoryModel
	{
		public string? Password { get; set; }
	}
}

public class TransferResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}