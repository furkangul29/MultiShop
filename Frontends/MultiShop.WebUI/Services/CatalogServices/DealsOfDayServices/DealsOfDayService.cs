using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices
    {
        public class DealsOfDayService : IDealsOfDayService
        {
            private readonly HttpClient _httpClient;

            public DealsOfDayService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task CreateDealAsync(CreateDealsOfDayDto createDealDto)
            {
                await _httpClient.PostAsJsonAsync<CreateDealsOfDayDto>("dealsoftheday", createDealDto);
            }

            public async Task DeleteDealAsync(string id)
            {
                await _httpClient.DeleteAsync("dealsoftheday?id=" + id);
            }

            public async Task<UpdateDealsOfDayDto> GetByIdDealAsync(string id)
            {
                var responseMessage = await _httpClient.GetAsync("dealsoftheday/" + id);
                var values = await responseMessage.Content.ReadFromJsonAsync<UpdateDealsOfDayDto>();
                return values;
            }

            public async Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync()
            {
                var responseMessage = await _httpClient.GetAsync("dealsoftheday");
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultDealsOfDayDto>>(jsonData);
                return values;
            }

            public async Task UpdateDealAsync(UpdateDealsOfDayDto updateDealDto)
            {
                await _httpClient.PutAsJsonAsync<UpdateDealsOfDayDto>("dealsoftheday", updateDealDto);
            }
        }
    }
