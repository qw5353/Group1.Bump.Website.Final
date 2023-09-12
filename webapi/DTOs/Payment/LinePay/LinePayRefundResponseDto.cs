using System.Text.Json.Serialization;

namespace webapi.DTOs.Payment.LinePay
{
    public class LinePayRefundResponseDto
    {
        public string? ReturnCode { get; set; }
        public string? ReturnMessage { get; set; }

        [JsonPropertyName("info")]
        public LinePayRefundInfo? Info { get; set; }
    }

    public class LinePayRefundInfo
    {
        public int? RefundTransactionId { get; set; }
        public string? RefundTransactionDate { get; set; }
    }
}
