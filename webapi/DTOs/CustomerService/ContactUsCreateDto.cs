using webapi.Models;

namespace webapi.DTOs.CustomerService
{
    public class ContactUsCreateDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Inquiry { get; set; }
    }

    public static class ContactUsExts
    {
        public static ContactU ToEntity(this ContactUsCreateDto dto)
        {
            return new ContactU
            {
                Id = 0,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Subject = dto.Subject,
                Inquiry = dto.Inquiry,
                Status = "待回覆",
                CreatedAt = DateTime.Now
            };
        }
    }
}
