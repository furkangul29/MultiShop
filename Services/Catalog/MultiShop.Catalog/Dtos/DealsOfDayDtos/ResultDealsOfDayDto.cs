namespace MultiShop.Catalog.Dtos.DealsOfDayDtos
{
    public class ResultDealsOfDayDto
    {
        public string DealsOfDayId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
