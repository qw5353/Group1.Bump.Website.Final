using webapi.DTOs.CustomerService;
using webapi.Models;

namespace webapi.DTOs.Shop
{
	public class ProductListDto
	{
		public List<ProductDto> Products { get; set; }

		public int TotalPage { get; set; } = 0;
	}

}
