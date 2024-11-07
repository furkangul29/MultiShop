namespace MultiShop.Catalog.Dtos.ProductDtos
{
    public class FavoriteToggleResponse
    {
        public bool Success { get; set; } 
        public int Count { get; set; } 
        public bool IsFavorited { get; set; }
    }
}
