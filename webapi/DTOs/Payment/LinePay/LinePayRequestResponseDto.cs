namespace webapi.DTOs.Payment.LinePay
{
    public class LinePayRequestResponseDto
    {
        public string? ReturnCode { get; set; }
        public string? ReturnMessage { get; set; }
        public Info? Info { get; set; }
    }

    public class Info
    {
        public Paymenturl? PaymentUrl { get; set; }
        public long TransactionId { get; set; }
        public string? PaymentAccessToken { get; set; }
    }

    public class Paymenturl
    {
        public string? Web { get; set; }
        public string? App { get; set; }
    }
}
