using webapi.Models;
using static Dapper.SqlMapper;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace webapi.DTOs.Coupon
{
    public class CouponSendMemberListDto
    {
        public int Id { get; set; }
        public string MemberName { get; set; }
        public string Account { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? SendingTime { get; set; }
        public bool Usage { get; set; }

        public int CouponId { get; set; }
        public string CouponName { get; set; }
        public string Code { get; set; }
        public string EventTypeName { get; set; }
        public DateTime? CouponStartTime { get; set; }
        public DateTime? CouponEndTime { get; set; }
        public string TargetTypeName { get; set; }
        public string PromotionProductTypeName { get; set; }
        public string Status { get; set; }
        public int PriceThreshold { get; set; }
        public string CouponTypeName { get; set; }
        public int? DiscountQty { get; set; }
        public decimal? Amount { get; set; }
        public string FreebieName { get; set; }
        public int? Quantity { get; set; }
        public bool ExtraSalesUsage { get; set; }
        public int MemberId { get; set; }
        public List<string> ProductTagNames { get; set; }

    }
    public static class CouponSendMemberExts
    {
        public static CouponSendMemberListDto ToDto(this CouponSendＭember entity)
        {
            return new CouponSendMemberListDto()
            {
                Id = entity.Id,
                MemberId = entity.MemberId,
                MemberName = entity.Member.Name,
                Account = entity.Member.Account,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                SendingTime = entity.StartTime,
                Usage = entity.Usage,
                CouponId = entity.Coupon.Id,
                CouponName = entity.Coupon.Name,
                Code = entity.Coupon.Code,
                EventTypeName = entity.Coupon.EventType.Name,
                CouponStartTime = entity.Coupon.StartTime,
                CouponEndTime = entity.Coupon.EndTime,
                TargetTypeName = entity.Coupon.TargetType.Name,
                PromotionProductTypeName = entity.Coupon.PromotionProductType?.Name??"",
                Status = entity.Coupon.Status,
                PriceThreshold = entity.Coupon.PriceThreshold,
                CouponTypeName = entity.Coupon.CouponType.Name,
                DiscountQty = entity.Coupon.DiscountQty,
                Amount = entity.Coupon.Amount,
                FreebieName = entity.Coupon.Freebie?.Name,
                Quantity = entity.Coupon.Quantity,
                ExtraSalesUsage = entity.Coupon.ExtraSalesUsage,
                ProductTagNames = entity.Coupon.ProductTags?.Select(pt=>pt.Name).ToList()
            };
        }
    }

}
