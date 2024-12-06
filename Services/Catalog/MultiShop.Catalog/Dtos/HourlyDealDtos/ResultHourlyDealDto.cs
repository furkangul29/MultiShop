namespace MultiShop.Catalog.Dtos.HourlyDealDtos
{
    public class ResultHourlyDealDto
    {
        public string HourlyDealId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CategoryName { get; set; }
    }
}
