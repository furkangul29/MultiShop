﻿namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class FavoriteProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
