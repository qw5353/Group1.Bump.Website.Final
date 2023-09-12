using webapi.Models;

namespace webapi.DTOs.CustomerService
{
    public class FAQListDto
    {
        public int Id { get; set; }
        public int QuestionTypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? QuestionType { get; set; }
    }
}
