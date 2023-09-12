using webapi.Models;

namespace webapi.DTOs.FieldService
{
    public class skillcourseDto
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
        
        public string? Thumbnail { get; set; }
        
 
    }
    public static class skillcourseExts
    {
        public static skillcourseDto ToDto(this SkillCourse db)
        {
            return new skillcourseDto
            {
                Id = db.Id,
                Name = db.Name,
                Thumbnail = db.Thumbnail
            };
        }
    } 
}
