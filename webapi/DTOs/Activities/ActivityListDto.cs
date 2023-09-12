using webapi.Models;

namespace webapi.DTOs.Activities
{
    public class ActivityListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public List<ActivityDetailsDto> ActivityDetails { get; set; }
    }

    public static class ActivityListDtoExts
    {
        public static ActivityListDto ToDto(this Activity entity)
        {
            return new ActivityListDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Description = entity.Description,
                Thumbnail = entity.Thumbnail,
                ActivityDetails = entity.ActivityDetails.Select(detail => detail.ToDto()).ToList()
			};
        }
    }
}
