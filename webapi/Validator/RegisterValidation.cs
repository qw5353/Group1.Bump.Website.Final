using FluentValidation;
using webapi.DTOs.Account;

namespace webapi.Validator
{
	public class RegisterValidation : AbstractValidator<RegisterDto>
	{
		public RegisterValidation()
		{
			RuleFor(r => r.Account).NotEmpty();
			RuleFor(r => r.Password).NotEmpty();
			RuleFor(r => r.ConfirmPassword).NotEmpty();
			RuleFor(r => r.Password).Equal(r => r.ConfirmPassword);
			RuleFor(r => r.Email).EmailAddress();
			RuleFor(r => r.Birthday).NotEmpty();
			RuleFor(r => r.PhoneNumber).NotEmpty();

		}
	}
}
