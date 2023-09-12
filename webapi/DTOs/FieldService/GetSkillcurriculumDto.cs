using webapi.Models;

namespace webapi.DTOs.FieldService
{
    public class GetSkillcurriculumDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        //public string? SkillCoursesName { get; set; }
        //public int CoachId { get; set; }
        public string? CoachName { get; set; }

        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public int PeopleMin { get; set; }
        public int PeopleMax { get; set; }
        public int Price { get; set; }
    }
    public static class GetSkillcurriculumExt
    {
        public static GetSkillcurriculumDto ToGetSkillcurriculumDto(this Skillcurriculum db)
        {
            return new GetSkillcurriculumDto
            {
                Id = db.Id,
                Name = db.Name,
                StartTime = db.StartTime,
                EndTime = db.EndTime,
                Description=db.SkillCourses.Description,
                Thumbnail=db.SkillCourses.Thumbnail,
                PeopleMax=db.SkillCourses.PeopleMax, 
                PeopleMin=db.SkillCourses.PeopleMin,
                Price=db.SkillCourses.Price,
                CoachName=db.Coach.Name,

            };
        }
    }
}
