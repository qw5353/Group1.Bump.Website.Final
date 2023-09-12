
using System.ComponentModel.DataAnnotations;
using webapi.Models;

namespace webapi.DTOs.FieldService
{
    public class ExperienceEnrollmentDto
    {
        //public int Id { get; set; }
        public int ExperienceCoursePlanId { get; set; }
        public string MemberId { get; set; }
        public int PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NumberOfPeople { get; set; }
        public bool Ststus { get; set; }
        public string FieldId { get; set; }
        public int? CoachId { get; set; }
       
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public static class ExperienceEnrollmentExt 
    { 
        public static ExperienceEnrollment ToEntity(this ExperienceEnrollmentDto dto)
        {
            return new ExperienceEnrollment
            {
                ExperienceCoursePlanId= dto.ExperienceCoursePlanId,
                MemberId = int.Parse(dto.MemberId),
                PaymentId = dto.PaymentId,
                CreatedAt = dto.CreatedAt,
                NumberOfPeople = int.Parse(dto.NumberOfPeople),
                Ststus = dto.Ststus,
                FieldId = int.Parse(dto.FieldId),
                CoachId=dto.CoachId,
                StartTime = DateTime.Parse(dto.StartTime), // 轉換成 DateTime
                EndTime = DateTime.Parse(dto.EndTime)
            };
        }
     }
        
}
