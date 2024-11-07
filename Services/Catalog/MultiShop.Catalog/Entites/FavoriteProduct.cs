namespace MultiShop.Catalog.Entites
{
    public class FavoriteProduct
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
