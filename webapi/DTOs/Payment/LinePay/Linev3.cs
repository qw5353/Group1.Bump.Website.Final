namespace webapi.DTOs.Payment.LinePay
{
    public class Linev3
    {
    }

    public class LineProduct
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }
    }

    public class LinePackage
    {
        public string Id => Guid.NewGuid().ToString();

        public int Amount { get; set; }

        public string Name { get; set; }

        public List<LineProduct> Products { get; set; }
    }

    public class LineRedirectUrls
    {
        public string ConfirmUrl => "https://localhost:5002/pay/line/confirm";

        public string CancelUrl => "https://localhost:5002/pay/line/cancel";
    }

    public class LineForm
    {
        public int Amount { get; set; }

        public string Currency => "TWD";

        public string OrderId { get; set; }

        public List<LinePackage> Packages { get; set; }

        public LineRedirectUrls RedirectUrls => new LineRedirectUrls();
    }
}
