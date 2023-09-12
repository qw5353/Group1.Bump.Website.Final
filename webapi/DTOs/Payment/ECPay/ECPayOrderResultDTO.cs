namespace webapi.DTOs.Payment.ECPay
{
    public class ECPayOrderResultDTO
    {
        public string? MerchantID { get; set; }
        public string? MerchantTradeNo { get; set; }
        public int RtnCode { get; set; }
    }
}
