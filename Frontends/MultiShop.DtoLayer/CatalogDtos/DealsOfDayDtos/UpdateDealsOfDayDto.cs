namespace MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos
{
    public class UpdateDealsOfDayDto
    {
        public string DealsOfDayId { get; set; }
        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
