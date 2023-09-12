using webapi.Models;

namespace webapi.DTOs.Activities
{
	public class ActivityDetailsDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
        public string Thumbnail { get; set; }
        public List<ActivityDiscountsDto> ActivityDiscounts { get; set; }
	}

    public static class ActivityDetailsDtoExts
    {
        public static ActivityDetailsDto ToDto(this ActivityDetail entity)
        {
            return new ActivityDetailsDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Description = entity.Description,
                Thumbnail = entity.Thumbnail,
                ActivityDiscounts = entity.ActivityDiscounts.Select(discount => discount.ToDiscountDto()).ToList()
            };
        }
    }
}