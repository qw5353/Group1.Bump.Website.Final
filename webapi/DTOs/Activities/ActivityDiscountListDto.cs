using webapi.Models;

namespace webapi.DTOs.Activities
{
    public class ActivityDiscountListDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public DateTime ActivityStartTime { get; set; }
        public DateTime ActivityEndTime { get; set; }
        public int AcitivityDetailId { get; set; }
        public string ActivityDetailName { get; set; }
        public DateTime ActivityDetailStartTime { get; set; }
        public DateTime ActivityDetailEndTime { get; set; }
        public string TargetTypeName { get; set; }
        public string PromotionProductTypeName { get; set; }
        public int PriceThreshold { get; set; }
        public string DiscountTypeName { get; set; }
        public int? DiscountQty { get; set; }
        public decimal? Amount { get; set; }
        public string FreebieName { get; set; }
        public string FreebieImg { get; set; }
        public string GiftCouponName { get; set; }
        public string ProductTagName { get; set; }
    }

    public static class ActivityDiscountListDtoExts
    {
        public static ActivityDiscountListDto ToDto(this ActivityDiscount entity)
        {
            return new ActivityDiscountListDto()
            {
                ActivityId = entity.AcitivityDetail.ActivityId,
                ActivityName = entity.AcitivityDetail.Activity.Name,
                ActivityStartTime = entity.AcitivityDetail.Activity.StartTime,
                ActivityEndTime = entity.AcitivityDetail.Activity.EndTime,
                AcitivityDetailId = entity.AcitivityDetail.Id,
                ActivityDetailName = entity.AcitivityDetail.Name,
                ActivityDetailStartTime = entity.AcitivityDetail.StartTime,
                ActivityDetailEndTime = entity.AcitivityDetail.EndTime,
                TargetTypeName = entity.TargetType.Name,
                PromotionProductTypeName = entity.PromotionProductType.Name,
                PriceThreshold = entity.PriceThreshold,
                DiscountTypeName = entity.DiscountType.Name,
                DiscountQty = entity.DiscountQty,
                Amount = entity.Amount,
                FreebieName = entity.Freebie?.Name,
                FreebieImg = entity.Freebie?.Thumbnail,
                GiftCouponName = entity.GiftCoupon?.Name,
                ProductTagName = entity.ProductTags?.Select(pt => pt.Name).FirstOrDefault()
            };
        }
    }
}
