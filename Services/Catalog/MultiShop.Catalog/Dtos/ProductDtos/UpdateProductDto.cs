﻿namespace MultiShop.Catalog.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryId { get; set; }
        public string Color { get; set; }
        public string? Size { get; set; }

        public bool InStock { get; set; }
    }
}
