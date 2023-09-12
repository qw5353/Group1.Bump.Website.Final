using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using System.Text.Json;
using System.Text;
using System.Web;
using webapi.DTOs.Payment.LinePay;
using webapi.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using webapi.DTOs.Payment.ECPay;

namespace webapi.Services
{
    public class PayService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public PayService(IConfiguration config, IHttpClientFactory clientFactory)
        {
            _configuration = config;
            _httpClientFactory = clientFactory;
        }

        public async Task<string?> LinePayRequest(LinePayRequestDto dto)
        {
            var httpClient = _httpClientFactory.CreateClient("LinePay");

            var nonce = Guid.NewGuid().ToString();
            var apiURL = "/v3/payments/request";
            var channelSecret = _configuration["LinePay:ChannelSecret"] ?? "";

            var payloadJson = JsonSerializer.Serialize(new LineForm
            {
                Amount = dto.Amount,
                OrderId = dto.OrderId,
                Packages = new List<LinePackage>()
                {
                    new LinePackage
                    {
                        Amount = dto.Amount,
                        Name = dto.ProductName ?? "Bump 商店",
                        Products = new List<LineProduct>()
                        {
                            new LineProduct
                            {
                                Name = dto.ProductName ?? "Bump 商店",
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
                return null;
            }

            var webPaymentUrl = responseDto?.Info?.PaymentUrl?.Web;

            if (webPaymentUrl == null)
            {
                return null;
            }

            return webPaymentUrl;
        }


        public async Task<bool> CheckECPayStatus(string merchantTradeNo)
        {
            var parameters = new SortedDictionary<string, string?>()
            {
                { "MerchantID", _configuration["ECPay:MerchantID"]},
                { "TimeStamp",  DateTimeOffset.Now.ToUnixTimeSeconds().ToString()},
                { "MerchantTradeNo", merchantTradeNo},
            };


            parameters["CheckMacValue"] = GetCheckMacValue(parameters);

            var httpClient = _httpClientFactory.CreateClient();

            using var httpResposeMessage = await httpClient.PostAsync("https://payment-stage.ecpay.com.tw/Cashier/QueryTradeInfo/V5", new FormUrlEncodedContent(parameters));

            httpResposeMessage.EnsureSuccessStatusCode();

            string result = await httpResposeMessage.Content.ReadAsStringAsync();

            string? tradeStatus = result.Split("&").FirstOrDefault(p => p.Contains("TradeStatus"))?.Split("=").Skip(1).Take(1).Single();

            if (tradeStatus == "1") return true;

            return false;
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

        public SortedDictionary<string, string?> PrepareForm(ECPayCheckoutDTO dto)
        {
            var parameters = new SortedDictionary<string, string?>()
            {
                { "MerchantID", _configuration["ECPay:MerchantID"]},
                { "MerchantTradeNo", dto.MerchantTradeNo},
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                { "PaymentType", "aio"},
                { "TotalAmount", dto.Amount.ToString() },
                { "TradeDesc", "Bump 線上銷售"},
                { "ItemName",  dto.Name},
                { "ReturnURL", "https://localhost:5002/api/pay/ECPay/return" },
                { "OrderResultURL", "https://localhost:5002/api/pay/ecpay/order_result" },
                { "ChoosePayment", "ALL" },
                { "EncryptType", "1" }
            };


            parameters["CheckMacValue"] = GetCheckMacValue(parameters);

            return parameters;
        }

        private string GetCheckMacValue(SortedDictionary<string, string?> parameters)
        {
            var hashKey = _configuration["ECPay:HashKey"];
            var hashIV = _configuration["ECPay:HashIV"];

            var joinedParams = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            var url = $"HashKey={hashKey}&{joinedParams}&HashIV={hashIV}";

            var checkMacValue = HttpUtility.UrlEncode(url).ToLower().ToSHA256String().ToUpper();

            return checkMacValue;
        }
    }
}
