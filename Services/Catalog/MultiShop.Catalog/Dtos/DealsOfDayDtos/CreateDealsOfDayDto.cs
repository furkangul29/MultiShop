﻿namespace MultiShop.Catalog.Dtos.DealsOfDayDtos
{
    public class CreateDealsOfDayDto
    {
        public string ProductId { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
