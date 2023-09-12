using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs.Cart;
using webapi.DTOs.Payment;
using webapi.DTOs.Payment.ECPay;
using webapi.Models;
using webapi.Repositories.DapperRepositories;
using webapi.Services;
using static Dapper.SqlMapper;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly CartRepository _cartRepository;
        private readonly BumpContext _context;
        private readonly PayService _payService;

        public CartController(CartRepository cartRepository, BumpContext context, PayService payService)
        {
            _cartRepository = cartRepository;
            _context = context;
            _payService = payService;
        }

        [HttpGet]
        [Route("/cart")]
        public async Task<ActionResult<IEnumerable<CartVM>>> Details(int id)
        {
            var detail = await _cartRepository.GetCartItems(id);

            return Ok(detail);
        }

        [HttpGet]
        [Route("/cart/memberInfo")]
        public async Task<ActionResult<MemberInfoVM>> MemberDetails(int id)
        {
            var detail = await _cartRepository.MemberInfo(id);

            return Ok(detail);
        }

        [HttpGet]
        [Route("/cart/memberPoint")]
        public async Task<ActionResult<MemberInfoVM>> MemberPoint(int id)
        {
            var detail = await _cartRepository.MemberRedPoint(id);

            return Ok(detail);
        }

        [HttpPost]
        [Route("/cart/CartDetailQuantity")]
        public async Task<ActionResult> CartDetailQuantity(CartDetailUpdateQuantityDto dto)
        {
            //await _cartRepository.UpdateCartDetailQuantity(id ,quantity);
            bool updateSuccess = await _cartRepository.UpdateCartDetailQuantity(dto.Id, dto.Quantity);

            if (updateSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest("更新購物車詳細資料數量失敗。");
            }
        }

        [HttpDelete]
        [Route("/cart/items/{id}")]
        public async Task<ActionResult> DeleteCartItem(int id)
        {
            bool deleteSuccess = await _cartRepository.DeleteCartItems(id);

            if (deleteSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest("更新購物車詳細資料數量失敗。");
            }

        }

        [HttpPost]
        [Route("checkout")]
        public async Task<ActionResult> Checkout(CheckoutVM checkoutData)
        {
            int orderId = await _cartRepository.CreateOrder(checkoutData);

            switch (checkoutData.PayType)
            {
                case "Line Pay":
                    string redirectUrl = await LinePayCheckout(checkoutData, orderId);
                    return Ok(redirectUrl);
                case "ECPay":
                    SortedDictionary<string, string?> form = await PrepareECPayForm(checkoutData, orderId);
                    return Ok(form);
                default:
                    return BadRequest();
            }
        }

        [HttpPost]
        [Route("re-checkout")]
        public async Task<IActionResult> ReCheckout(ReCheckoutDto dto)
        {
            var orderId = dto.OrderId;
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var payment = await _context.Payments.SingleAsync(p => p.BumpOrderId == orderId);

            switch (payment.PayType)
            {
                case "Line Pay":
                    string redirectUrl = await LinePayReCheckout(payment, orderId);
                    await _context.SaveChangesAsync();
                    return Ok(new { PayType = "Line Pay", RedirectUrl = redirectUrl });
                case "綠界支付":
                    SortedDictionary<string, string?> form = await RePrepareECPayForm(payment, orderId);
                    return Ok(new { PayType = "ECPay", Form = form });
                default:
                    return BadRequest();
            }
        }

        private async Task<SortedDictionary<string, string?>> RePrepareECPayForm(Payment payment, int bumpOrderId)
        {
            var merchantTradeNo = payment.Id.ToString().PadLeft(8, '0') + DateTimeOffset.Now.ToUnixTimeSeconds().ToString().PadLeft(12, '0');
            _context.Entry(payment).State = EntityState.Modified;
            _context.Entry(payment).CurrentValues.SetValues(new { ECPayMerchantTradeNo = merchantTradeNo });
            await _context.SaveChangesAsync();

            var form = _payService.PrepareForm(new ECPayCheckoutDTO
            {
                MerchantTradeNo = merchantTradeNo,
                Amount = payment.Amount,
                Name = "Bump 戶外用品"
            });

            return form;
        }

        private async Task<SortedDictionary<string, string?>> PrepareECPayForm(CheckoutVM checkoutData, int bumpOrderId)
        {

            var entity = _context.Payments.Add(new Payment()
            {
                Amount = checkoutData.TotalProductsPrice,
                PayType = "綠界支付",
                Status = "未付款",
                BumpOrderId = bumpOrderId,
            });


            int affectedRows = await _context.SaveChangesAsync();

            if (affectedRows == 0) throw new Exception("無法結帳");

            var merchantTradeNo = entity.Entity.Id.ToString().PadLeft(8, '0') + DateTimeOffset.Now.ToUnixTimeSeconds().ToString().PadLeft(12, '0');
            entity.State = EntityState.Modified;
            entity.CurrentValues.SetValues(new { ECPayMerchantTradeNo = merchantTradeNo });
            await _context.SaveChangesAsync();

            var form = _payService.PrepareForm(new ECPayCheckoutDTO
            {
                MerchantTradeNo = merchantTradeNo,
                Amount = checkoutData.TotalProductsPrice,
                Name = "Bump 戶外用品"
            });

            return form;
        }

        private async Task<string> LinePayReCheckout(Payment payment, int orderId)
        {
            string lineOrderId = Guid.NewGuid().ToString();
            payment.LineOrderId = lineOrderId;

            var linepayCheckoutUrl = await _payService.LinePayRequest(new DTOs.Payment.LinePay.LinePayRequestDto
            {
                Amount = payment.Amount,
                OrderId = lineOrderId,
                ConfirmUrl = "https://localhost:5002/api/confirm/linepay",
                ProductName = "Bump 戶外用品"
            });

            return linepayCheckoutUrl;
        }

        private async Task<string> LinePayCheckout(CheckoutVM checkoutData, int orderId)
        {
            string lineOrderId = Guid.NewGuid().ToString();

            var entity = _context.Payments.Add(new Payment()
            {
                Amount = checkoutData.TotalProductsPrice,
                LineOrderId = lineOrderId,
                PayType = "Line Pay",
                Status = "未付款",
                BumpOrderId = orderId,
            });

            int affectedRows = await _context.SaveChangesAsync();

            if (affectedRows == 0) throw new Exception("無法結帳");

            var linepayCheckoutUrl = await _payService.LinePayRequest(new DTOs.Payment.LinePay.LinePayRequestDto
            {
                Amount = checkoutData.TotalProductsPrice,
                OrderId = lineOrderId,
                ConfirmUrl = "https://localhost:5002/api/confirm/linepay",
                ProductName = "Bump 戶外用品"
            });

            return linepayCheckoutUrl;
        }
    }
}
