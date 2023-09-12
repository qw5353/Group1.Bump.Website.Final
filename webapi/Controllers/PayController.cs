using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System;
using System.Buffers.Text;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using webapi.DTOs.Payment.ECPay;
using webapi.DTOs.Payment.LinePay;
using webapi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly BumpContext _context;
        
        public PayController(IHttpClientFactory clientFactory, IConfiguration config, BumpContext context)
        {
            _httpClientFactory = clientFactory;
            _config = config;
            _context = context;
        }

        [HttpPost]
        [Route("line/request")]
        public async Task<IActionResult> LinePayRequest(LinePayRequestDto dto)
        {
            var httpClient = _httpClientFactory.CreateClient("LinePay");

            var nonce = Guid.NewGuid().ToString();
            var apiURL = "/v3/payments/request";
            var channelSecret = _config["LinePay:ChannelSecret"] ?? "";

            var payloadJson = JsonSerializer.Serialize(new LineForm
            {
                Amount = dto.Amount,
                Packages = new List<LinePackage>()
                {
                    new LinePackage
                    {
                        Amount = dto.Amount,
                        Name = "Bump 戶外用品",
                        Products = new List<LineProduct>()
                        {
                            new LineProduct
                            {
                                Name = "Bump 戶外用品",
                                Quantity = 1,
                                Price = dto.Amount,
                            }
                        }
                    }
                }

            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var payRequestJson = new StringContent(
                payloadJson,
                Encoding.UTF8,
                Application.Json
            );

            httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", CreateSignPost(nonce, apiURL, channelSecret, payloadJson));

            using var httpResponseMessage = await httpClient.PostAsync(apiURL, payRequestJson);

            httpResponseMessage.EnsureSuccessStatusCode();

            var responseDto = await httpResponseMessage.Content.ReadFromJsonAsync<LinePayRequestResponseDto>();

            if (responseDto?.ReturnCode != "0000")
            {
                return BadRequest();
            }

            var webPaymentUrl = responseDto?.Info?.PaymentUrl?.Web;

            if (webPaymentUrl == null)
            {
                return BadRequest();
            }

            return Ok(webPaymentUrl);
        }

        [HttpPost]
        [Route("line/confirm")]
        public async Task<IActionResult> ConfirmLinePay(LinePayConfirmDto dto)
        {
            var httpClient = _httpClientFactory.CreateClient("LinePay");

            var nonce = Guid.NewGuid().ToString();
            var apiURL = $"/v3/payments/{dto.TransactionId}/confirm";
            var channelSecret = _config["LinePay:ChannelSecret"] ?? "";


            var payloadJson = JsonSerializer.Serialize(new { dto.Amount, dto.Currency }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var payRequestJson = new StringContent(
                payloadJson,
                Encoding.UTF8,
                Application.Json
            );

            httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", CreateSignPost(nonce, apiURL, channelSecret, payloadJson));


            using var httpResponseMessage = await httpClient.PostAsync(apiURL, payRequestJson);

            httpResponseMessage.EnsureSuccessStatusCode();


            var responseDto = await httpResponseMessage.Content.ReadFromJsonAsync<LinePayConfirmResponseDto>();

            if (responseDto?.ReturnCode != "0000")
            {
                return Problem("支付未成功", statusCode: 402);
            }

            var isPaid = await CheckLinePayStatus(dto.TransactionId);

            if (!isPaid)
            {
               return Problem("支付未成功", statusCode: 402);
            }

            var payment = await _context.Payments.AsQueryable().SingleAsync(p => p.LineOrderId == dto.LineOrderId);
            payment.Status = "已付款";

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("line/order/{lineOrderId}")]
        public async Task<IActionResult> GetLineOrderAmount(string lineOrderId)
        {
            var payment = await _context.Payments.AsQueryable().FirstOrDefaultAsync(p => p.LineOrderId == lineOrderId);

            if (payment == null) return NotFound();

            return Ok(payment.Amount);
        }

        private async Task<bool> CheckLinePayStatus(string? transactionId)
        {
            var httpClient = _httpClientFactory.CreateClient("LinePay");

            var nonce = Guid.NewGuid().ToString();
            var apiURL = $"/v3/payments/requests/{transactionId}/check";
            var channelSecret = _config["LinePay:ChannelSecret"] ?? "";


            httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", CreateSignGet(nonce, apiURL, channelSecret));


            using var httpResponseMessage = await httpClient.GetAsync(apiURL);

            httpResponseMessage.EnsureSuccessStatusCode();

            var responseDto = await httpResponseMessage.Content.ReadFromJsonAsync<LinePayStatusDto>();

            if (responseDto?.ReturnCode != "0123")
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        [Route("ecpay/order_result")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmECPayStatus([FromForm]ECPayOrderResultDTO dto)
        {
            var payment = await _context.Payments.SingleAsync(p => p.ECPayMerchantTradeNo == dto.MerchantTradeNo);

            payment.Status = "已付款";
            await _context.SaveChangesAsync();

            return Redirect("https://localhost:5002/Member/Orders");
        }


        private string CreateSignPost(string nonceStr, string url, string secret, string body)
        {
            var hmac = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(secret + url + body + nonceStr));
            return Convert.ToBase64String(hmac);
        }

        private string CreateSignGet(string nonceStr, string url, string secret, string body = "")
        {
            var hmac = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(secret + url + body + nonceStr));
            return Convert.ToBase64String(hmac);
        }

    }
}
