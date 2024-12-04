using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniKPay.RestApi.Features.User;

[Table("TBL_User")]
public class UserModel
{
	[Key]
	[Column("UserId")]
	public string? Id { get; set; }
	[Column("UserName")]
	public string? Name { get; set; }
	[Column("UserMobileNo")]
	public string? MobileNo { get; set; }
	[Column("UserPassword")]
	public string? Password { get; set; }
	[Column("UserBalance")]
	public decimal Balance { get; set; }
}

public class UserResponseModel
{
	public bool IsSuccessful { get; set; }
	public string? Message { get; set; }
}