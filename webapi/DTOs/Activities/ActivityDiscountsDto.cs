using webapi.Models;

namespace webapi.DTOs.Activities
{
	public class ActivityDiscountsDto
	{
		public int Id { get; set; }
		public string PromotionProductTypeName { get; set; }
        public int PriceThreshold { get; set; }
        public List<ProductTagsDto> ProductTags { get; set; }
	}
	public static class ActivityDiscountsDtoExts
	{
		public static ActivityDiscountsDto ToDiscountDto(this ActivityDiscount entity)
		{
			return new ActivityDiscountsDto
			{
				Id = entity.Id,
				PromotionProductTypeName = entity.PromotionProductType.Name,
				PriceThreshold = entity.PriceThreshold,
				ProductTags = entity.ProductTags.Select(p=>p.ToDto()).ToList()
			};
		}
	}

	public class ProductTagsDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	public static class ProductTagsDtoExts
	{
		public static ProductTagsDto ToDto(this ProductTag entity)
		{
			return new ProductTagsDto
			{
				Id =entity.Id,
				Name =entity.Name
			};
		}
	}
}
