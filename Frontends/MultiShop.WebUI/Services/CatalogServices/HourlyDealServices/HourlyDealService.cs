using MultiShop.DtoLayer.CatalogDtos.HourlyDealDtos;
using Newtonsoft.Json;
using System;
using System.Web.Helpers;

namespace MultiShop.WebUI.Services.CatalogServices.HourlyDealServices
{
    public class HourlyDealService : IHourlyDealService
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "hourlydeals";

        public HourlyDealService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultHourlyDealDto>> GetCurrentHourlyDealsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/current");
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ResultHourlyDealDto>>(jsonData)
                    ?? new List<ResultHourlyDealDto>();
            }
            catch (HttpRequestException ex)
            {
                // Daha detaylı hata yönetimi
                Console.Error.WriteLine($"Saatlik güncel indirimler alınırken hata: {ex.Message}");
                return new List<ResultHourlyDealDto>();
            }
        }

        public async Task<List<ResultHourlyDealDto>> GenerateHourlyDealsAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseEndpoint}/generate", null);
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ResultHourlyDealDto>>(jsonData)
                    ?? new List<ResultHourlyDealDto>();
            }
            catch (HttpRequestException ex)
            {
                // Daha detaylı hata yönetimi
           
                Console.Error.WriteLine($"Saatlik indirimler oluşturulurken hata: {ex.Message}");
                return new List<ResultHourlyDealDto>();
            }
        }

        
        public async Task DeactivateExpiredHourlyDealsAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseEndpoint}/deactivate", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Süresi geçen indirimler temizlenirken hata: {ex.Message}");
            }
        }

        public async Task<List<DateTime>> GetHourlyDealEndTimesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/endtimes");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();


                //  DateTime listesi olarak deserialize et.  
                //  Backend'in DateTime'ları ISO 8601 formatında gönderdiğinden emin olun.
                return JsonConvert.DeserializeObject<List<DateTime>>(jsonString);


            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Saatlik indirimlerin bitiş zamanları alınırken hata oluştu: {ex.Message}");
                return new List<DateTime>(); // Boş liste döndür
            }
            catch (JsonSerializationException ex) // JSON ayrıştırma hatası yakalama
            {
                Console.Error.WriteLine($"JSON ayrıştırma hatası: {ex.Message}");
                return new List<DateTime>();
            }
        }
    }
}