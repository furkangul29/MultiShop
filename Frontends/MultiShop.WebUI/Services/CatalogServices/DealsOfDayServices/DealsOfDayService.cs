using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices
{

    public class DealsOfDayService : IDealsOfDayService
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "dealsofdays";

        public DealsOfDayService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync()
        {
            try
            {
                var responseMessage = await _httpClient.GetAsync(BaseEndpoint);
                responseMessage.EnsureSuccessStatusCode();
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultDealsOfDayDto>>(jsonData);
                return values ?? new List<ResultDealsOfDayDto>();
            }
            catch (HttpRequestException ex)
            {
                // Log the error or handle it appropriately
                throw new Exception("Failed to fetch deals of the day", ex);
            }
        }

        public async Task CreateDealAsync(CreateDealsOfDayDto createDealDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, createDealDto);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to create deal", ex);
            }
        }

        public async Task UpdateDealAsync(UpdateDealsOfDayDto updateDealDto)
        {
            try
            {
                var url = $"{BaseEndpoint}/UpdateDealsOfDay/{updateDealDto.DealsOfDayId}";


                var response = await _httpClient.PutAsJsonAsync(url, updateDealDto);
                var content = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to update deal: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteDealsOfDayAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");


                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to delete deal. Error: {errorMessage}, Status Code: {response.StatusCode}"); // Include status code for better debugging
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete deal due to an HTTP error.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while trying to delete the deal.", ex);
            }
        }
        public async Task<ResultDealsOfDayDto> GetByIdDealAsync(string id)
        {
            try
            {
                var responseMessage = await _httpClient.GetAsync($"{BaseEndpoint}/{id}");
                responseMessage.EnsureSuccessStatusCode();
                var values = await responseMessage.Content.ReadFromJsonAsync<ResultDealsOfDayDto>();
                return values ?? throw new Exception("Deal not found");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to fetch deal by id", ex);
            }
        }

        public async Task<ResultDealsOfDayDto> UpdateGetByIdDealAsync(string id)
        {
            try
            {
                // API'ye GET isteği gönder
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/{id}/UpdateGetByIdDealAsync");
                response.EnsureSuccessStatusCode(); // Hata durumunda bir istisna atar

                // Yanıtı DTO'ya dönüştür
                var result = await response.Content.ReadFromJsonAsync<ResultDealsOfDayDto>();
                return result ?? throw new Exception("Deal not found");
            }
            catch (HttpRequestException ex)
            {
                // Hata durumunda loglama yapılabilir
                throw new Exception("Failed to fetch deal by id", ex);
            }
        }
        public async Task ChangeDealStatusAsync(string dealId, bool isActive)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/{dealId}/ChangeStatus", isActive);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to change deal status", ex);
            }
        }

    }
}