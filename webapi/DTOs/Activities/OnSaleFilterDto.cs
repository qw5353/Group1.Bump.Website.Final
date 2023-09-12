namespace webapi.DTOs.Activities
{
    public class OnSaleFilterDto
    {
        public List<string>? ProductTagNames { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? FirstCategory { get; set; }

        public List<string>? SecondCategories { get; set; }

        public List<string>? ThirdCategories { get; set; }

        public List<string>? BrandName { get; set; }

        public List<string>? ProductStyle { get; set; }

        public int? MinPrice { get; set; }

        public int? MaxPrice { get; set; }

    }
}
