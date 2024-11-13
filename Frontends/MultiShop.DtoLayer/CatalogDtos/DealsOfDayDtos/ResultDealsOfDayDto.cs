namespace MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos
{
    public class ResultDealsOfDayDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
    }
}
