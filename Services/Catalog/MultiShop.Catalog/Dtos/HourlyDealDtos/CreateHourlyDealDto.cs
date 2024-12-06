namespace MultiShop.Catalog.Dtos.HourlyDealDtos
{
    public class CreateHourlyDealDto
    {
        public string ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
