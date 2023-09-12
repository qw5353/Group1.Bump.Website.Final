namespace webapi.DTOs.Payment.ECPay
{
    public class ECPayCheckoutDTO
    {
        public int Amount { get; set; }
        public string? Name { get; set; }
        public string? MerchantTradeNo { get; set; }
    }
}
