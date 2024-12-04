using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniKPay.RestApi.Features.User;

[Table("TBL_User")]
public class UserModel
{
	[Key]
	public string? UserId { get; set; }
	public string? UserName { get; set; }
	public string? UserMobileNo { get; set; }
	public string? UserPassword { get; set; }
	public decimal UserBalance { get; set; }
}

public class UserResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}