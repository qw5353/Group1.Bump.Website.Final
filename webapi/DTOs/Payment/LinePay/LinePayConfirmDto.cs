namespace webapi.DTOs.Payment.LinePay
{
    public class LinePayConfirmDto
    {
        public string? TransactionId { get; set; }
        public int Amount { get; set; }
        public string? Currency => "TWD";
        public string? LineOrderId { get; set; }
    }
}
