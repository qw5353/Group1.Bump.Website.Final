using webapi.Models;

namespace webapi.DTOs.Coupon
{
    public class CouponProductListDto
    {
        public int Id { get; set; }
        public int ThirdCategoryId { get; set; }
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Price { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateTime { get; set; }
        public string ShelfStatus { get; set; }

    }

    public static class CouponProductListDtoExts
    {
        public static CouponProductListDto ToDto(this Product entity)
        {
            return new CouponProductListDto
            {
                Id = entity.Id,
                ThirdCategoryId = entity.ThirdCategoryId,
                BrandId = entity.BrandId,
                Name = entity.Name,
                Code = entity.Code,
                Price = entity.Price,
                ShortDescription = entity.ShortDescription,
                Description = entity.Description,
                Thumbnail = entity.Thumbnail
            };
        }
    }
}
