namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class ProductFiltersDto
    {
        public string? CategoryId { get; set; }

        // Birden fazla fiyat aralığı seçimi için
        public List<PriceRangesDto>? SelectedPrices { get; set; }

        // Renk seçimi için
        public List<string>? SelectedColors { get; set; }

        // Beden seçimi için
        public List<string>? SelectedSizes { get; set; }
    }

    // Fiyat aralığı DTO'su
    public class PriceRangesDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
