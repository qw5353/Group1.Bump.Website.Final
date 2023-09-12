using FluentValidation;
using webapi.DTOs.Account;

namespace webapi.Validator
{
	public class ForgetPwdValidator :AbstractValidator<ForgetPasswordDto>
	{
		public ForgetPwdValidator() 
		{
			RuleFor(r => r.Password).NotEmpty();
			RuleFor(r => r.ConfirmPassword).NotEmpty();
			RuleFor(r => r.Password).Equal(r => r.ConfirmPassword);
		}
	}
}
