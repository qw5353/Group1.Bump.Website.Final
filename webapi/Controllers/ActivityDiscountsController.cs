using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Crypto;
using Swashbuckle.AspNetCore.Filters;
using webapi.DTOs.Activities;
using webapi.DTOs.Shop;
using webapi.Models;
using webapi.Repositories.DapperRepositories;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActivityDiscountsController : ControllerBase
    {
        private readonly BumpContext _context;

        public ActivityDiscountsController(BumpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetDiscount")]
        public async Task<ActionResult<List<Discount>>> GetDiscount(CartActivityDiscountsDto dto)
        {
            var bestDiscounts = await GetBestDiscount(dto);
            var discountsCompsFreebie = bestDiscounts.Select(group =>
            group.Select(subgroup =>
            subgroup.Where(item => item.ActivityDiscount.DiscountTypeName == "贈品").ToList())
            .Where(subgroup => subgroup.Count > 0).ToList())
                .Where(group => group.Count > 0).ToList();
            List<bool> containsGiftDiscount = new List<bool>();

            var discountsCompsPrice = bestDiscounts.Select(g => g.Select(g => g.Sum(g => g.TotalDiscountPrice)).ToList()).ToList();
            var maxDiscount = discountsCompsPrice.Where(d => d.Count() > 0).Select(d => d.Max()).DefaultIfEmpty(0).Max();

            var itemsMaxDiscount = bestDiscounts.Select(g => g.Where(g => g.Sum(g => g.TotalDiscountPrice) == maxDiscount).ToList()).ToList();

            var maxCount = itemsMaxDiscount.Select(i => i.SelectMany(i => i).Count()).DefaultIfEmpty(0).Max();
            var maxCountElement = itemsMaxDiscount.FirstOrDefault(i => i.SelectMany(i => i).Count() == maxCount);

            var flattenMaxDiscount = new List<Discount>();

            if (maxCountElement != null)
            {
                flattenMaxDiscount = maxCountElement
                    .SelectMany(innerList => innerList)
                    .ToList();
            }

            return flattenMaxDiscount;
        }

        private async Task<List<List<List<Discount>>>> GetBestDiscount(CartActivityDiscountsDto dto)
        {
            var items = dto.CartVM;
            var activityDiscounts = await CartActivityDiscount(dto);
            var freebieDiscounts = activityDiscounts.Where(ad => ad.ActivityDiscount.DiscountTypeName == "贈品").ToList();
            activityDiscounts = activityDiscounts.Except(freebieDiscounts).ToList();
            
            var discountComps = new List<List<List<Discount>>>();

            foreach (var discount in activityDiscounts)
            {
                bool isContain = items.Intersect(discount.DiscountProducts).Count() > 0;
                if (isContain)
                {
                    var otherProducts = new CartVM[] { };

                    otherProducts = items.Except(discount.DiscountProducts).ToArray();
                    
                    var index = activityDiscounts.IndexOf(discount);

                    int j = 0;
                    while (discountComps.Count <= index)
                    {
                        discountComps.Add(new List<List<Discount>>());
                    }

                    while (discountComps[index].Count <= j)
                    {
                        discountComps[index].Add(new List<Discount>());
                    }
                    discountComps[index][j].Add(discount);

                    if (otherProducts.Length > 0)
                    {
                        dto.CartVM = otherProducts;
                        var otherActivityDiscounts = await CartActivityDiscount(dto);
                        otherActivityDiscounts = otherActivityDiscounts.Where(ad => ad.ActivityDiscount.AcitivityDetailId != discount.ActivityDiscount.AcitivityDetailId).ToList();
                        otherActivityDiscounts = otherActivityDiscounts.Where(ad => ad.ActivityDiscount.DiscountTypeName != "贈品").ToList();

                        for (int i = 0; i < otherActivityDiscounts.Count(); i++)
                        {
                            var otherDiscount = otherActivityDiscounts[i];
                            bool isContainOther = false;
                            if (otherDiscount.DiscountProducts.Count() != 0)
                            {
                                isContainOther = otherProducts.Intersect(otherDiscount.DiscountProducts).Count() > 0;
                            }

                            if (isContainOther)
                            {
                                var anotherProducts = new CartVM[] { };
                                anotherProducts = items;

                                while (discountComps.Count <= index)
                                {
                                    discountComps.Add(new List<List<Discount>>());
                                }

                                while (discountComps[index].Count <= j)
                                {
                                    discountComps[index].Add(new List<Discount>());
                                }
                                discountComps[index][j].Add(otherDiscount);

                                if (anotherProducts.Length == 0)
                                {
                                    j++;
                                    while (discountComps.Count <= index)
                                    {
                                        discountComps.Add(new List<List<Discount>>());
                                    }

                                    while (discountComps[index].Count <= j)
                                    {
                                        discountComps[index].Add(new List<Discount>());
                                    }
                                    discountComps[index][j].Add(discount);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var comp in discountComps)
            {
                foreach (var sublist in comp)
                {
                    sublist.AddRange(freebieDiscounts);
                }
            }

            return discountComps;
        }


        private async Task<List<Discount>> CartActivityDiscount(CartActivityDiscountsDto dto)
        {
            var memberId = int.Parse(User.FindFirst("memberId").Value);

            var items = dto.CartVM;
            var member = await _context.Members.Where(m => m.Id == memberId).Include(m => m.MemberLevel).FirstAsync();
            var discounts = await _context.ActivityDiscounts
                .Where(ad => ad.AcitivityDetail.StartTime <= DateTime.Now && ad.AcitivityDetail.EndTime >= DateTime.Now)
                .Include(ad => ad.AcitivityDetail)
                .ThenInclude(at => at.Activity)
                .Include(ad => ad.PromotionProductType)
                .Include(ad => ad.DiscountType)
                .Include(ad => ad.TargetType)
                .Include(ad => ad.MemberLevels)
                .Include(ad => ad.Freebie)
                .Include(ad => ad.ProductTags)
                .ThenInclude(p => p.Products)
                .Include(ad => ad.ThirdCategories)
                .ToListAsync();
            var activityDiscount = new Discount();
            var activityDiscounts = new List<Discount>();

            var productsInCart = items;

            if (!discounts.Any())
            {
                activityDiscount.TotalDiscountPrice = 0;
                activityDiscount.ActivityDiscount = null;
                activityDiscounts.Add(activityDiscount);
            }
            else
            {
                foreach (var discount in discounts)
                {
                    var activityDetailName = discount.AcitivityDetail.Name;
                    var targetTypeName = discount.TargetType.Name;
                    var promotionTypeName = discount.PromotionProductType.Name;
                    var discountTypeName = discount.DiscountType.Name;
                    var priceThreshold = discount.PriceThreshold;

                    activityDiscount = new Discount();
                    var totalDiscountPrice = 0;
                    var discountPerPrices = new List<int>();

                    var freebieName = string.Empty;
                    var freebieImg = string.Empty;

                    var productConditions = new List<bool>();
                    bool memberCondition = false;

                    var discountProducts = new List<CartVM>();
                    switch (targetTypeName)
                    {
                        case "會員等級":
                            var memberLevels = discount.MemberLevels.Select(ml => ml.Name).ToList();
                            memberCondition = memberLevels.Contains(member.MemberLevel.Name);
                            break;
                        case "會員標籤":
                            var memberTags = discount.MemberTags.Select(ml => ml.Name).ToList();
                            memberCondition = memberTags.Intersect(member.MemberTags.Select(m => m.Name)).Any();
                            break;
                        case "全部會員":
                            memberCondition = true;
                            break;
                    }

                    foreach (var product in productsInCart)
                    {
                        switch (promotionTypeName)
                        {
                            case "商品標籤":
                                var productTags = discount.ProductTags.ToList();
                                bool isProductTags = productTags.Any(pt => pt.Products.Select(p => p.Name).Contains(product.Name));
                                productConditions.Add(isProductTags);
                                if (isProductTags) discountProducts.Add(product);
                                break;
                            case "商品種類":
                                var productCategories = discount.ThirdCategories.Where(p => p.Products.Select(p => p.Name).Contains(product.Name));
                                bool isProductCategories = productCategories != null;
                                productConditions.Add(isProductCategories);
                                if (isProductCategories) discountProducts.Add(product);
                                break;
                            case "全館商品":
                                productConditions.Add(true);
                                discountProducts.Add(product);
                                break;
                        }
                    }

                    if (productConditions.Any(condition => condition) && memberCondition)
                    {
                        int totalDiscountProducts = discountProducts.Sum(fp => fp.Price * fp.Quantity);
                        var discountPerProducts = discountProducts.Select(fp => fp.Price * fp.Quantity).ToList();

                        if (totalDiscountProducts >= priceThreshold)
                        {
                            switch (discountTypeName)
                            {
                                case "打折":
                                    totalDiscountPrice = (int)(totalDiscountProducts * (1 - discount.Amount ?? 0));
                                    discountPerPrices = discountPerProducts.Select(dp => (int)(dp * (1 - discount.Amount ?? 0))).ToList();
                                    break;

                                case "折抵":
                                    totalDiscountPrice = (int)(discount.Amount ?? 0);

                                    var sumDiscountPerProducts = discountPerProducts.Sum();
                                    discountPerPrices = discountPerProducts.Select(dp => (int)(totalDiscountPrice * dp / sumDiscountPerProducts)).ToList();

                                    break;

                                case "贈品":
                                    freebieName = discount.Freebie.Name;
                                    freebieImg = discount.Freebie.Thumbnail;
                                    break;
                            }
                        }
                    }

                    activityDiscount.TotalDiscountPrice = totalDiscountPrice;
                    activityDiscount.DiscountProducts = discountProducts;
                    activityDiscount.DiscountPerPrices = discountPerPrices;
                    activityDiscount.ProductConditions = productConditions;
                    activityDiscount.ActivityDiscount = discount.ToDto();
                    activityDiscounts.Add(activityDiscount);
                }
            }

            return activityDiscounts;
        }

        private async Task<List<Discount>> ProductActivityDiscount(Member member, Product product, List<ActivityDiscount> discounts, ProductActivityDiscountDto dto)
        {
            var activityDiscount = new Discount();
            var activityDiscounts = new List<Discount>();

            if (!discounts.Any())
            {
                activityDiscount.TotalDiscountPrice = 0;
                activityDiscount.ActivityDiscount = null;
                activityDiscounts.Add(activityDiscount);
            }
            else
            {
                foreach (var discount in discounts)
                {
                    var activityDetailName = discount.AcitivityDetail.Name;
                    var targetTypeName = discount.TargetType.Name;
                    var promotionTypeName = discount.PromotionProductType.Name;
                    var discountTypeName = discount.DiscountType.Name;
                    var priceThreshold = discount.PriceThreshold;

                    activityDiscount = new Discount();
                    var totalDiscountPrice = 0;

                    var freebieName = string.Empty;
                    var freebieImg = string.Empty;

                    var perProductConditions = false;
                    bool memberCondition = false;

                    switch (targetTypeName)
                    {
                        case "會員等級" when member.Account != null:
                            var memberLevels = discount.MemberLevels.Select(ml => ml.Name).ToList();
                            memberCondition = memberLevels?.Contains(member.MemberLevel.Name) ?? false;
                            break;
                        case "會員標籤" when member.Account != null:
                            var memberTags = discount.MemberTags.Select(ml => ml.Name).ToList();
                            memberCondition = memberTags?.Intersect(member.MemberTags.Select(m => m.Name)).Any() ?? false;
                            break;
                        case "全部會員":
                            memberCondition = true;
                            break;
                    }

                    switch (promotionTypeName)
                    {
                        case "商品標籤":
                            var productTags = discount.ProductTags.ToList();
                            bool isProductTags = productTags.Any(pt => pt.Products.Select(p => p.Id).Contains(product.Id));
                            perProductConditions = isProductTags;
                            break;
                        case "商品種類":
                            var productCategories = _context.ThirdCategories.Where(p => p.Products.Contains(product));
                            bool isProductCategories = productCategories != null;
                            perProductConditions = isProductCategories;
                            break;
                        case "全館商品":
                            perProductConditions = true;
                            break;
                    }

                    if (priceThreshold == 0 && perProductConditions && memberCondition)
                    {
                        switch (discountTypeName)
                        {
                            case "打折":
                                totalDiscountPrice = (int)(product.Price * (1 - (discount.Amount ?? 0)));
                                break;

                            case "折抵":
                                totalDiscountPrice = (int)(discount.Amount ?? 0);
                                break;

                            case "贈品":
                                freebieName = discount.Freebie.Name;
                                freebieImg = discount.Freebie.Thumbnail;
                                break;
                        }
                    }
                    activityDiscount.TotalDiscountPrice = totalDiscountPrice;
                    activityDiscount.ActivityDiscount = discount.ToDto();
                    activityDiscount.PerProductConditions = perProductConditions;
                    activityDiscounts.Add(activityDiscount);
                }
            }

            return activityDiscounts;
        }

        [HttpPost]
        [Route("GetProductDiscount")]
        public async Task<ActionResult<List<Discount>>> GetProductDiscount(ProductActivityDiscountDto dto)
        {
            var member = new Member();
            var memberIdClaim = User.FindFirst("memberId");
            var memberIdStr = memberIdClaim != null ? memberIdClaim.Value : string.Empty;
            int? memberId = memberIdStr == "" ? null : int.Parse(memberIdStr);
            if (memberId != null) member = await _context.Members.Include(m=>m.MemberLevel).FirstAsync(m => m.Id == memberId);

            var product = await _context.Products.FirstAsync(m => m.Id == dto.ProductId);

            var discounts = await _context.ActivityDiscounts
                .Where(ad => ad.AcitivityDetail.StartTime <= DateTime.Now && ad.AcitivityDetail.EndTime >= DateTime.Now)
                .Include(ad => ad.AcitivityDetail)
                .ThenInclude(at => at.Activity)
                .Include(ad => ad.PromotionProductType)
                .Include(ad => ad.DiscountType)
                .Include(ad => ad.TargetType)
                .Include(ad => ad.MemberLevels)
                .Include(ad => ad.Freebie)
                .Include(ad => ad.ProductTags)
                .ThenInclude(p => p.Products)
                .Include(ad => ad.ThirdCategories)
                .ToListAsync();
            var discountsList = await ProductActivityDiscount(member, product, discounts, dto);
            var maxDiscountPrice = (discountsList.Select(d => d.TotalDiscountPrice)).Max();

            var activityDiscounts = discountsList.Where(d => d.PerProductConditions != false).ToList();
            //var freebieDiscounts = discounts.Where(d => d.ActivityDiscount.DiscountTypeName == "贈品" && d.PerProductConditions != false).ToList();

            // var redPointsDiscounts = discounts.Where(d => d.ActivityDiscount.DiscountTypeName == "點數加倍送").ToList();

            return activityDiscounts;
        }

        //促銷中 幻燈片
        [HttpGet]
        [Route("OnSaleProductGroup")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetOnSaleProductGroup()
        {
            var member = new Member();
            var memberIdClaim = User.FindFirst("memberId");
            var memberIdStr = memberIdClaim != null ? memberIdClaim.Value : string.Empty;
            int? memberId = memberIdStr == "" ? null : int.Parse(memberIdStr);
            if (memberId != null) member = await _context.Members.Include(m => m.MemberLevel).FirstAsync(m => m.Id == memberId);

            var discounts = await _context.ActivityDiscounts
                .Where(ad => ad.AcitivityDetail.StartTime <= DateTime.Now && ad.AcitivityDetail.EndTime >= DateTime.Now)
                .Include(ad => ad.AcitivityDetail)
                .ThenInclude(at => at.Activity)
                .Include(ad => ad.PromotionProductType)
                .Include(ad => ad.DiscountType)
                .Include(ad => ad.TargetType)
                .Include(ad => ad.MemberLevels)
                .Include(ad => ad.Freebie)
                .Include(ad => ad.ProductTags)
                .ThenInclude(p => p.Products)
                .Include(ad => ad.ThirdCategories)
                .ToListAsync();
            var products = await _context.Products.Include(p => p.Brand).ToListAsync();
            var productIds = products.Select(p => p.Id).ToList();

            var groupDto = new List<PrudocutGroup>();
            foreach (var product in products)
            {
                var discountDto = new ProductActivityDiscountDto { ProductId = product.Id };
                var discountsList = await ProductActivityDiscount(member, product, discounts, discountDto);
                var maxDiscountPrice = (discountsList.Select(d => d.TotalDiscountPrice)).Max();

                groupDto.Add(new PrudocutGroup()
                {
                    ProductId = product.Id,
                    MaxDiscountPrice = maxDiscountPrice
                });
            }

            var topDiscounts = groupDto
                .OrderByDescending(g => g.MaxDiscountPrice != 0)
                .Take(12)
                .ToList();

            var productIdsWithTopDiscounts = topDiscounts
                .Select(g => g.ProductId)
                .ToList();

            var productDict = products.ToDictionary(p => p.Id);

            var productDtos = productIdsWithTopDiscounts
                .Select(productId => productDict.GetValueOrDefault(productId))
                .Where(product => product != null)
                .Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    BrandName = product.Brand.Name,
                    Price = product.Price,
                    Thumbnail = product.Thumbnail
                })
                .ToList();

            return productDtos;
        }

        [HttpGet]
        [Route("ActUsedProductTags")]
        public async Task<ActionResult<IEnumerable<ActivityDetailsDto>>> GetActUsedProductTags()
        {
            var activities = await _context.ActivityDetails
                .Where(ad => ad.StartTime <= DateTime.Now.AddDays(7) && ad.EndTime >= DateTime.Now)
                .Include(ad => ad.ActivityDiscounts)
                .ThenInclude(adc => adc.PromotionProductType)
                .Include(ad => ad.ActivityDiscounts)
                .ThenInclude(adc => adc.ProductTags)
                .Where(ad => ad.ActivityDiscounts.All(adc => adc.ProductTags.Any()))
                .ToListAsync();

            return activities.Select(a => a.ToDto()).ToList();
        }
    }

    public class PrudocutGroup
    {
        public int ProductId { get; set; }
        public int MaxDiscountPrice { get; set; }
    }

    public class Discount
    {
        public int TotalDiscountPrice { get; set; }
        public int MaxDiscountPrice { get; set; }
        public List<CartVM> DiscountProducts { get; set; }
        public List<int> DiscountPerPrices { get; set; }
        public List<bool> ProductConditions { get; set; }
        public bool PerProductConditions { get; set; }
        public ActivityDiscountListDto ActivityDiscount { get; set; }
    }

}
