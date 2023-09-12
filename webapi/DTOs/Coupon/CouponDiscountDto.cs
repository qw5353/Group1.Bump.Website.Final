using webapi.Models;

namespace webapi.DTOs.Coupon
{
    public class CouponDiscountDto
    {
        public List<string> CouponTypeName { get; set; }
        public List<int> DiscountPrice { get; set; }
        public List<string> FreebieName { get; set; }
        public List<CouponSendＭember> Coupons { get; set; }

    }
}
