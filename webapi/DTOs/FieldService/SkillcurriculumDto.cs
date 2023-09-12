using webapi.Models;

namespace webapi.DTOs.FieldService
{
    public class SkillcurriculumDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int FieldId { get; set; }
        public string? FieldName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte Week { get; set; }
        public string? StartDate { get; set; }
        public string? SkillCoursesName { get; set; }
    }
    public static class SkillcurriculumExt
    {
        public static SkillcurriculumDto ToDto(this Skillcurriculum db)
        {
            return new SkillcurriculumDto
            {
                Id = db.Id,
                Name = db.Name,
                FieldId = db.FieldId,
                StartTime = db.StartTime,
                EndTime = db.EndTime,
                Week = db.Week,
                StartDate = db.StartDate.ToString("yyyy/MM/dd"),
                SkillCoursesName = db.SkillCourses.Name,
                FieldName =db.Field.Name
            };
        }
    }
}
