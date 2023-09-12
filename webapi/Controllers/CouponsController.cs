using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
using webapi.Repositories.DapperRepositories;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using webapi.DTOs.Coupon;
using webapi.DTOs.Activities;
using Microsoft.Identity.Client;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Hangfire;
using webapi.Helpers;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly BumpContext _context;
        private readonly HangfireHelper _hangfireHelper;
        public CouponsController(BumpContext context, HangfireHelper hangfireHelper)
        {
            _context = context;
            _hangfireHelper = hangfireHelper;
        }

        [HttpPost]
        [Route("CreateCouponSend")]
        public async Task<ActionResult<CouponSendＭember>> CreateCouponSendＭember(CouponSendMemberCreateDto dto)
        {
            var memberId = int.Parse(User.FindFirst("memberId").Value);

            var coupon = await _context.Coupons.Where(c => c.Code == dto.Code).FirstOrDefaultAsync();
            if (coupon == null) { return BadRequest("優惠券歸戶失敗！ 請檢查序號是否輸入錯誤"); }

            var couponInMember = await _context.CouponSendＭembers.Where(c => c.CouponId == coupon.Id && c.MemberId == memberId).ToListAsync();
            if (couponInMember.Count() != 0) { return BadRequest("優惠券歸戶失敗！ 您已領過此張優惠券囉"); }


            var couponSendMember = new CouponSendＭember()
            {
                MemberId = memberId,
                CouponId = coupon.Id,
                StartTime = coupon.StartTime == null ? DateTime.Now : ((DateTime)coupon.StartTime),
                EndTime = coupon.EndTime == null ? DateTime.Now : ((DateTime)coupon.EndTime),
                SendingTime = coupon.StartTime == null ? DateTime.Now : ((DateTime)coupon.StartTime),
                Usage = false
            };
            _context.CouponSendＭembers.Add(couponSendMember);
            await _context.SaveChangesAsync();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(couponSendMember, settings);

            return CreatedAtAction("GetCouponSendＭember", new { memberId = couponSendMember.MemberId }, json);
        }

        [HttpPost]
        [Route("TransferCouponSend")]
        public async Task<ActionResult<CouponSendＭember>> TransferCouponSendＭember(CouponSendMemberTransferDto dto)
        {
            if (dto.Account == string.Empty || dto.Email == string.Empty) return BadRequest("空值");
            var member = await _context.Members.Where(m => m.Account == dto.Account && m.Email == dto.Email).ToListAsync();
            if (member.Count == 0) { return BadRequest("優惠券移轉失敗！ 請檢查轉贈帳號及Email是否存在"); }

            var memberId = member.First().Id;

            var couponSendMember = await _context.CouponSendＭembers.FindAsync(dto.SendId);
            if (couponSendMember == null) { return BadRequest("找不到優惠券"); }

            _context.CouponSendＭembers.Remove(couponSendMember);

            var couponId = couponSendMember.CouponId;
            var couponForMember = await _context.CouponSendＭembers.Where(cm => cm.CouponId == couponId && cm.MemberId == memberId).ToListAsync();
            if (couponForMember.Count() != 0) { return BadRequest("優惠券移轉失敗！ 您的贈送對象已有相同優惠券"); }

            couponSendMember = new CouponSendＭember()
            {
                MemberId = memberId,
                CouponId = couponSendMember.CouponId,
                StartTime = couponSendMember.StartTime,
                EndTime = couponSendMember.EndTime,
                SendingTime = couponSendMember.StartTime,
                Usage = false
            };
            _context.CouponSendＭembers.Add(couponSendMember);
            await _context.SaveChangesAsync();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(couponSendMember, settings);

            return CreatedAtAction("GetCouponSendＭember", new { memberId = couponSendMember.MemberId }, json);
        }

        [HttpGet]
        [Route("GetCouponProducts")]
        public async Task<ActionResult<IEnumerable<CouponProductListDto>>> GetCouponProducts(int sendId)
        {
            var couponSendMember = await _context.CouponSendＭembers.Where(cm => cm.Id == sendId).FirstOrDefaultAsync();
            if (couponSendMember == null) { return BadRequest("找不到優惠券"); }

            var couponId = couponSendMember.CouponId;
            var coupon = await _context.Coupons
                .Where(c => c.Id == couponId)
                .Include(c => c.ProductTags)
                .ThenInclude(pt => pt.Products)
                .FirstOrDefaultAsync();
                
            if (coupon == null) { return BadRequest("找不到優惠券"); }


            var products = coupon.ProductTags.Select(pt => pt.Products).SelectMany(p => p).Select(p => p.ToDto()).ToList();

            return products;
        }

        [HttpGet]
        [Route("MemberCoupons")]
        public async Task<ActionResult<IEnumerable<CouponSendMemberListDto>>> GetCouponSendＭember()
        {
            var memberId = int.Parse(User.FindFirst("memberId").Value);
            var couponSendＭember = await _context.CouponSendＭembers
                .Where(cm => cm.MemberId == memberId && !cm.Usage)
                .Where(cm => cm.StartTime <= DateTime.Now && cm.EndTime >= DateTime.Now && cm.SendingTime <= DateTime.Now)
                .Include(cm => cm.Coupon.CouponType)
                .Include(cm => cm.Coupon.EventType)
                .Include(cm => cm.Coupon.Freebie)
                .Include(cm => cm.Coupon.PromotionProductType)
                .Include(cm => cm.Coupon.TargetType)
                .Include(cm => cm.Coupon.ActivityCoupons)
                .Include(cm => cm.Coupon.MemberLevels)
                .Include(cm => cm.Coupon.MemberTags)
                .Include(cm => cm.Coupon.ProductTags)
                .Include(cm => cm.Coupon.ThirdCategories)
                .Include(cm => cm.Member)
                .Select(cm => cm.ToDto())
                .AsNoTracking()
                .ToListAsync();

            if (couponSendＭember.Count() == 0)
            {
                return NotFound();
            }

            return couponSendＭember;
        }

        [HttpPost]
        [Route("CartCoupons")]
        public async Task<ActionResult<CouponDiscount>> CartCouponDiscount(CartVM[] items)
        {
            var memberId = int.Parse(User.FindFirst("memberId").Value);

            var discounts = await _context.CouponSendＭembers
                .Where(cm => cm.MemberId == memberId && !cm.Usage)
                .Where(cm => cm.StartTime <= DateTime.Now && cm.EndTime >= DateTime.Now)
                .Include(cm => cm.Coupon.CouponType)
                .Include(cm => cm.Coupon.EventType)
                .Include(cm => cm.Coupon.Freebie)
                .Include(cm => cm.Coupon.PromotionProductType)
                .Include(cm => cm.Coupon.TargetType)
                .Include(cm => cm.Coupon.ActivityCoupons)
                .Include(cm => cm.Coupon.MemberLevels)
                .Include(cm => cm.Coupon.MemberTags)
                .Include(cm => cm.Coupon.ProductTags)
                .ThenInclude(pt => pt.Products)
                .Include(cm => cm.Coupon.ThirdCategories)
                .Include(cm => cm.Member)
                .ToListAsync();

            var productsInCart = items;
            var couponDiscount = new CouponDiscount();
            var discountPrices = new List<int>();
            var coupons = new List<CouponSendＭember>();

            if (!discounts.Any())
            {
                couponDiscount.DiscountPrice = new List<int> { 0 };
            }
            else
            {
                foreach (var discount in discounts)
                {
                    var couponName = discount.Coupon.Name;
                    var promotionTypeName = discount.Coupon.PromotionProductType?.Name ?? "";
                    var couponTypeName = discount.Coupon.CouponType.Name;
                    var priceThreshold = discount.Coupon.PriceThreshold;

                    var discountPrice = 0;
                    var freebieName = string.Empty;
                    var targetConditions = new List<bool>();

                    var discountProducts = new List<CartVM>();
                    foreach (var product in productsInCart)
                    {
                        switch (promotionTypeName)
                        {
                            case "商品標籤":
                                var productTags = discount.Coupon.ProductTags.ToList();
                                bool isProductTags = productTags.Any(pt => pt.Products.Select(p => p.Name).Contains(product.Name));
                                targetConditions.Add(isProductTags);
                                if (isProductTags) discountProducts.Add(product);
                                break;
                            case "商品種類":
                                var productCategories = _context.ThirdCategories.Where(p => p.Products.Select(p => p.Name).Contains(product.Name));
                                bool isProductCategories = productCategories != null;
                                targetConditions.Add(isProductCategories);
                                if (isProductCategories) discountProducts.Add(product);
                                break;
                            case "全館商品":
                                targetConditions.Add(true);
                                discountProducts.Add(product);
                                break;
                        }
                    }
                    if (targetConditions.Any(condition => condition))
                    {
                        //int totalDiscountProducts = discountProducts?.Sum(p => p.Price) ?? 0;
                        var filteredProducts = items.Where(item => discountProducts.Contains(item)).ToList();
                        int totalDiscountProducts = filteredProducts?.Sum(p => p.Price * p.Quantity) ?? 0;
                        if (totalDiscountProducts >= priceThreshold)
                        {
                            switch (couponTypeName)
                            {
                                case "打折":
                                    var value = totalDiscountProducts * (1 - discount.Coupon.Amount);
                                    discountPrice = value == null ? 0 : (int)value;
                                    break;

                                case "折抵":
                                    discountPrice = (int)discount.Coupon.Amount;
                                    break;

                                case "贈品":
                                    freebieName = discount.Coupon.Freebie.Name;
                                    break;
                            }
                            discountPrices.Add(discountPrice);
                            coupons.Add(discount);
                        }

                    }
                }
                couponDiscount.DiscountPrice = discountPrices;
                couponDiscount.Coupons = coupons.Select(c => c.ToDto()).ToList();
            }
            return couponDiscount;
        }

        [HttpPost]
        [Route("ProductCoupons")]
        public async Task<ActionResult<CouponDiscount>> ProductCouponDiscount(ProductActivityDiscountDto dto)
        {
            var memberIdClaim = User.FindFirst("memberId");
            var memberIdStr = memberIdClaim != null ? memberIdClaim.Value : string.Empty;
            int? memberId = memberIdStr == "" ?  null : int.Parse(memberIdStr);
            if (memberId == null) return new CouponDiscount();

            var product = await _context.Products.FindAsync(dto.ProductId);
            var discounts = await _context.CouponSendＭembers
                .Where(cm => cm.MemberId == memberId && !cm.Usage)
                .Where(cm => cm.StartTime <= DateTime.Now && cm.EndTime >= DateTime.Now)
                .Include(cm => cm.Coupon.CouponType)
                .Include(cm => cm.Coupon.EventType)
                .Include(cm => cm.Coupon.Freebie)
                .Include(cm => cm.Coupon.PromotionProductType)
                .Include(cm => cm.Coupon.TargetType)
                .Include(cm => cm.Coupon.ActivityCoupons)
                .Include(cm => cm.Coupon.MemberLevels)
                .Include(cm => cm.Coupon.MemberTags)
                .Include(cm => cm.Coupon.ProductTags)
                .ThenInclude(pt => pt.Products)
                .Include(cm => cm.Coupon.ThirdCategories)
                .Include(cm => cm.Member)
                .ToListAsync();

            var productsInCart = product;
            var couponDiscount = new CouponDiscount();
            var discountPrices = new List<int>();
            var coupons = new List<CouponSendＭember>();

            if (!discounts.Any())
            {
                couponDiscount.DiscountPrice = new List<int> { 0 };
            }
            else
            {
                foreach (var discount in discounts)
                {
                    var couponName = discount.Coupon.Name;
                    var promotionTypeName = discount.Coupon.PromotionProductType?.Name ?? "";
                    var couponTypeName = discount.Coupon.CouponType.Name;
                    var priceThreshold = discount.Coupon.PriceThreshold;

                    var discountPrice = 0;
                    var freebieName = string.Empty;
                    var targetConditions = false;

                    var discountProducts = new List<CartVM>();

                    switch (promotionTypeName)
                    {
                        case "商品標籤":
                            var productTags = discount.Coupon.ProductTags.ToList();
                            bool isProductTags = productTags.Any(pt => pt.Products.Select(p => p.Name).Contains(product.Name));
                            targetConditions = isProductTags;
                            break;
                        case "商品種類":
                            var productCategories = _context.ThirdCategories.Where(p => p.Products.Select(p => p.Name).Contains(product.Name));
                            bool isProductCategories = productCategories != null;
                            targetConditions = isProductCategories;
                            break;
                        case "全館商品":
                            targetConditions = true;
                            break;
                    }
                    if (targetConditions == true)
                    {
                        discountPrices.Add(discountPrice);
                        coupons.Add(discount);
                    }
                }
                couponDiscount.DiscountPrice = discountPrices;
                couponDiscount.Coupons = coupons.Select(c => c.ToDto()).ToList();
            }
            return couponDiscount;
        }


        [HttpGet]
        [Route("ScheduledSendCoupon")]
        public async Task<ActionResult> ScheduledSendCoupon()
        {
            var birdayCoupons = await _context.Coupons
                .Include(c => c.EventType)
                .Where(c => c.EventType.Name == "生日禮")
                .ToListAsync();
            var activityCoupons = await _context.Coupons
                .Where(c => c.StartTime <= DateTime.Now && c.EndTime >= DateTime.Now)
                .Include(c => c.EventType)
                .Where(c => c.EventType.Name == "活動")
                .ToListAsync();

            // 生日禮發送
            foreach (var coupon in birdayCoupons)
            {
                var id = coupon.Id;
                var couponName = coupon.Name;
                var jobName = couponName;
                RecurringJob.AddOrUpdate(jobName, () => _hangfireHelper.SendBirthdayCoupons(id), Cron.Monthly(31, 16));
            }

            // 活動優惠券發送
            foreach (var coupon in activityCoupons)
            {
                var id = coupon.Id;
                var couponName = coupon.Name;
                var jobName = couponName;
                RecurringJob.AddOrUpdate(jobName, () => _hangfireHelper.SendActivityCoupons(id), Cron.Daily(16));
            }

            return Ok();
        }
    }
}

public class CouponDiscount
{
    public List<int> DiscountPrice { get; set; }
    public List<CouponSendMemberListDto> Coupons { get; set; }
}

