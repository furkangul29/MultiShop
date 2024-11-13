namespace MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos
{
    public class CreateDealsOfDayDto
    {
        public string ProductId { get; set; }

        public int DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}
