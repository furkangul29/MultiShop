namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class ProductFilterDto
    {
        public string? CategoryId { get; set; }

        // Birden fazla fiyat aralığı seçimi için
        public List<PriceRangeDto>? SelectedPrices { get; set; }

        // Renk seçimi için
        public List<string>? SelectedColors { get; set; }

        // Beden seçimi için
        public List<string>? SelectedSizes { get; set; }
    }

    // Fiyat aralığı DTO'su
    public class PriceRangeDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
