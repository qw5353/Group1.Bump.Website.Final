using webapi.DTOs.CustomerService;
using webapi.Models;

namespace webapi.DTOs.Shop
{
	public class ProductDto
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? BrandName { get; set; }
		public int Price { get; set; }
		public string? Thumbnail { get; set; }
	}

}
