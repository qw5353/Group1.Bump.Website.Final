namespace webapi.DTOs.Shop
{
	public class AddCartDto
	{
		public int? MemberId { get; set; }
		public int? ProductStyleId { get; set; }
		public int? AddProductStyleQuantity { get; set; }
	}
}
