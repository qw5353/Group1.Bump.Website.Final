using webapi.DTOs.CustomerService;
using webapi.Models;

namespace webapi.DTOs.Shop
{
	public class ProductDetailsDto
	{
		public int Id { get; set; }
		public int? BrandId { get; set; }
		public string? BrandThumbnail { get; set; }

		public string? BrandDescription { get; set; }
		public string? ProductThumbnail { get; set; }
		public string? Name { get;set; }
		public string? Code { get; set; }	
		public int? Price { get; set; }
		public string? ShortDescription { get; set; }
		public string? Description { get; set; }
	}

}
