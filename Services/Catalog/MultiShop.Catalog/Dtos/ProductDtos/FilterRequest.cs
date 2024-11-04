namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class FilterRequest
    {
        public string CategoryId { get; set; }
        public List<string> SelectedColors { get; set; }
        public List<string> SelectedSizes { get; set; }
        public List<PriceRange> SelectedPrices { get; set; }
    }

    public class PriceRange
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
