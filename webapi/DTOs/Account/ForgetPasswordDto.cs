using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Account
{
	public class ForgetPasswordDto
	{
		public int Id { get; set; }
		//[Required]
		//[DataType(DataType.Password)]
		public string Password { get; set; }
		//[Required]
		//[DataType(DataType.Password)]
		//[Compare(nameof(Password), ErrorMessage = ("輸入不正確"))]
		public string ConfirmPassword { get; set; }
		public string ConfirmCode { get; set; }

	}
}
