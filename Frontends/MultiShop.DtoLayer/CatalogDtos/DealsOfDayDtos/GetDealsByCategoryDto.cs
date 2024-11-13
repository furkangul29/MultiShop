namespace MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos
{
    public class GetDealsByCategoryDto
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ResultDealsOfDayDto> DealsOfDayItems { get; set; }
    }
}
