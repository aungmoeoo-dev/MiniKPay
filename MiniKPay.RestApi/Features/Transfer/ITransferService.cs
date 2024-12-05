namespace MiniKPay.RestApi.Features.Transfer
{
	public interface ITransferService
	{
		TransferResponseModel Transfer(TransferModel requestModel);
	}
}