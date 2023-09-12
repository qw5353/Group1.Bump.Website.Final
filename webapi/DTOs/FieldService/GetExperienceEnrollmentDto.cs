using webapi.Models;
namespace webapi.DTOs.FieldService
{
    public class GetExperienceEnrollmentDto
    {
        public int Id { get; set; }
        public string? MemberName { get; set; }
        public string? FieldName { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? CreatedAt { get; set; }
        public int NumberOfPeople { get; set; }
    }
    public static class GetExperienceEnrollmentExt
    {
        public static GetExperienceEnrollmentDto ToDto(this ExperienceEnrollment db)
        {
            return new GetExperienceEnrollmentDto
            {
                Id = db.Id,
                MemberName=db.Member.Name,
                FieldName= db.Field.Name,
                NumberOfPeople=db.NumberOfPeople,
                StartTime = db.StartTime.ToString("yyyy-MM-dd HH:mm"),
                EndTime = db.EndTime.ToString("yyyy-MM-dd HH:mm"),
                CreatedAt = db.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
