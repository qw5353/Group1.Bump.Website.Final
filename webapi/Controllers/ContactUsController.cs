using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using webapi.DTOs.CustomerService;
using webapi.Helpers;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly BumpContext _context;
        private readonly IValidator<ContactUsCreateDto> _validator;
        private readonly IEmailHelper _emailHelper;
        public ContactUsController(BumpContext context, IValidator<ContactUsCreateDto> validator, IEmailHelper emailHelper)
        {
            _context = context;
            _validator = validator;
            _emailHelper = emailHelper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddContactUs(ContactUsCreateDto dto)
        {
            ValidationResult result = await _validator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(Results.ValidationProblem(result.ToDictionary()));
            }

            _context.ContactUs.Add(dto.ToEntity());
            await _context.SaveChangesAsync();

            _emailHelper.SendEmail(new Email
            {
                Receiver = dto.Email,
                Subject = "Bump 已收到您的聯繫, 我們將盡速處理您的問題",
                Body = "感謝您將您寶貴的意見與Bump分享\n我們將在兩個工作日內回覆您\n謝謝您的耐心等待配合!",
            });

            return Ok();
        }
        
    }
}
