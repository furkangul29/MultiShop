using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiShop.DtoLayer.CatalogDtos.ProductDtos
{
    public class ResultProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryId { get; set; }

        // Yeni özellikler
        public double AverageRating { get; set; } // Dinamik rating hesaplaması
        public int ReviewCount { get; set; } // Yorum sayısı
        public decimal? DiscountedPrice { get; set; } // Yeni property
    }
}
