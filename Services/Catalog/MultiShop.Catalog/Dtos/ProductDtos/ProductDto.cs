namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool InStock { get; set; }
        // Diğer gerekli özellikler buraya eklenebilir
    }
}