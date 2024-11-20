using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;
using Newtonsoft.Json;

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
                var response = await _httpClient.PutAsJsonAsync(BaseEndpoint, updateDealDto);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to update deal", ex);
            }
        }

        public async Task DeleteDealAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseEndpoint}?id={id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete deal", ex);
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

    }
}