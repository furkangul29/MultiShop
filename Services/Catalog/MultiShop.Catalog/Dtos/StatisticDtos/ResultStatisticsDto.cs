namespace MultiShop.Catalog.Dtos.StatisticDtos
{
    public class ResultStatisticsDto
    {
        public long? BrandCount { get; set; }
        public long? CategoryCount { get; set; }
        public long? ProductCount { get; set; }
        public decimal? ProductAvgPrice { get; set; }
        public string MaxPriceProductName { get; set; }
        public string MinPriceProductName { get; set; }
    }

}
