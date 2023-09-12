using FluentValidation;
using webapi.DTOs.CustomerService;

namespace webapi.Validator
{
    public class ContactUsValidator : AbstractValidator<ContactUsCreateDto>
    {
        public ContactUsValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.Subject).NotEmpty();
            RuleFor(x => x.Inquiry).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
        }
    }
}
