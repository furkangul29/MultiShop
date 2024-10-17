namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class ProductFilterDto
    {
        public string CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Color { get; set; }  // Renk filtresi eklendi
        public string Size { get; set; }   // Boyut filtresi eklendi
    }
}
