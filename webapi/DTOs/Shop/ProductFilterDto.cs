using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Shop
{
	public class ProductFilterDto
	{
		public string? Keyword { get; set; }
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 5;
		public string? FirstCategory { get; set; }

		public List<string>? SecondCategories { get; set; }

		public List<string>? ThirdCategories { get; set; }

		public List<string>? BrandName { get; set; }

		public List<string>? ProductStyle { get; set; }

		public int? MinPrice { get; set; }

		public int? MaxPrice { get; set; }

		public int OrderKey { get; set; } = 0;

	}
}
