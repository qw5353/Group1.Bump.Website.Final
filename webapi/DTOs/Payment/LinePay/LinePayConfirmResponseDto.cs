using System.Text.Json.Serialization;

namespace webapi.DTOs.Payment.LinePay
{

    public class LinePayConfirmResponseDto
    {
        public string? ReturnCode { get; set; }
        public string? ReturnMessage { get; set; }
        [JsonPropertyName("info")]
        public ConfirmInfo? Info { get; set; }
        
        public class ConfirmInfo
        {
            public long TransactionId { get; set; }
            public string? OrderId { get; set; }
            public Payinfo[]? PayInfo { get; set; }
        }

        public class Payinfo
        {
            public string? Method { get; set; }
            public int Amount { get; set; }
            public string? MaskedCreditCardNumber { get; set; }
        }
    }





}
