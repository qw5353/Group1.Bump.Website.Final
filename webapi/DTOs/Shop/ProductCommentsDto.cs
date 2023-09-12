namespace webapi.DTOs.Shop
{
	public class ProductCommentsDto
	{ 
        public int? Rating { get; set; }
		public string? Description { get; set; }
		public string? CreateAt { get; set; }
        public string? ProductStyle { get; set; }
        public string? Account { get; set; }
		public string? MemberThumbnail { get; set; }
    }
}
