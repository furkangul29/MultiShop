using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MultiShop.WebUI.Services.StatisticServices.CatalogStatisticServices
{
    public class CatalogStatisticService : ICatalogStatisticService
    {
        private readonly HttpClient _httpClient;

        public CatalogStatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResultStatisticsDto> GetAllStatistics()
        {
            var responseMessage = await _httpClient.GetAsync("Statistics/GetAllStatistics");
            var result = await responseMessage.Content.ReadFromJsonAsync<ResultStatisticsDto>();
            Console.WriteLine("Sonuclar: ",result);
            return result;
        }

        // Existing methods can be kept for backward compatibility or specific needs
        public async Task<long> GetBrandCount()
        {
            var statistics = await GetAllStatistics();
            return statistics.BrandCount;
        }

        public async Task<long> GetCategoryCount()
        {
            var statistics = await GetAllStatistics();
            return statistics.CategoryCount;
        }

        public async Task<string> GetMaxPriceProductName()
        {
            var statistics = await GetAllStatistics();
            return statistics.MaxPriceProductName;
        }

        public async Task<string> GetMinPriceProductName()
        {
            var statistics = await GetAllStatistics();
            return statistics.MinPriceProductName;
        }

        //public async Task<decimal> GetProductAvgPrice()
        //{
        //    var statistics = await GetAllStatistics();
        //    return statistics.ProductAvgPrice;
        //}

        public async Task<long> GetProductCount()
        {
            var statistics = await GetAllStatistics();
            return statistics.ProductCount;
        }
    }

    public class ResultStatisticsDto
    {
        public long BrandCount { get; set; }
        public long CategoryCount { get; set; }
        public long ProductCount { get; set; }
        //public decimal ProductAvgPrice { get; set; }
        public string MaxPriceProductName { get; set; }
        public string MinPriceProductName { get; set; }
    }
}