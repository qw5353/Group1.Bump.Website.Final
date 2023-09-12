using webapi.Models;
namespace webapi.DTOs.FieldService
{
    public class SkillEnrollmentDto
    {
        public int SkillcurriculumsId { get; set; }
        public int MemberId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NumberOfPeople { get; set; }
        public string? PayType { get; set; }
        public int Amount { get; set; }
    }
    public static class SkillEnrollmentExt
    {
        public static SkillEnrollment ToEntity(this SkillEnrollmentDto dto)
        {
            return new SkillEnrollment
            {
                SkillcurriculumsId = dto.SkillcurriculumsId,
                MemberId = dto.MemberId,
                CreatedAt = dto.CreatedAt,
                NumberOfPeople = dto.NumberOfPeople,
            };
        }
    }
}
