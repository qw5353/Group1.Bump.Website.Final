using webapi.Models;

namespace webapi.DTOs.FieldService
{
    public class GetSkillEnrollmentDto
    {
        public int Id { get; set; }
        //public int SkillcurriculumsId { get; set; }
        public string? SkillcurriculumsName { get; set; }
        public int MemberId { get; set; }
        public string? CreatedAt { get; set; }
        public int NumberOfPeople { get; set; }
        public EnrollPaymentDto? Payment { get; set; }
    }

    public class EnrollPaymentDto
    {
        public int Id { get; set; }
        
        public int Amount { get; set; }
        public string? PayType { get; set; }
        public string? Status { get; set; }
    }
    public static class GetSkillEnrollmentExt
    {
        public static GetSkillEnrollmentDto ToDto(this SkillEnrollment db)
        {
            return new GetSkillEnrollmentDto
            {
                Id = db.Id,
                SkillcurriculumsName = db.Skillcurriculums.Name,
                MemberId = db.MemberId,
                CreatedAt = db.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                NumberOfPeople = db.NumberOfPeople,
                Payment = db.Payments.Select(p => new EnrollPaymentDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PayType = p.PayType,
                    Status = p.Status,
                }).FirstOrDefault()
            };
        }
    }
}
