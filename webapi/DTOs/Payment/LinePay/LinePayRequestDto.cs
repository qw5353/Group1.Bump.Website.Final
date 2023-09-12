namespace webapi.DTOs.Payment.LinePay
{
    public class LinePayRequestDto
    {
        public int Amount { get; set; }
        public string? ProductName { get; set; }
        public string? ConfirmUrl { get; set; }
        public string? OrderId { get; set; }
        public string? Currency => "TWD";
        public string? ConfirmUrlType => "SERVER";
    }
}
