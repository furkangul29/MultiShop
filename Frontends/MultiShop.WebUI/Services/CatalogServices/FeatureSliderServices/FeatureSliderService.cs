using MultiShop.DtoLayer.CatalogDtos.FeatureSliderDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices
{
    public class FeatureSliderService : IFeatureSliderService
    {
        private readonly HttpClient _httpClient;
        public FeatureSliderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task CreateFeatureSliderAsync(CreateFeatureSliderDto createFeatureSliderDto)
        {
            await _httpClient.PostAsJsonAsync<CreateFeatureSliderDto>("featuresliders", createFeatureSliderDto);
        }
        public async Task DeleteFeatureSliderAsync(string id)
        {
            await _httpClient.DeleteAsync("featuresliders?id=" + id);
        }
        public async Task<UpdateFeatureSliderDto> GetByIdFeatureSliderAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync("featuresliders/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<UpdateFeatureSliderDto>();
            return values;
        }
        public async Task<List<ResultFeatureSliderDto>> GetAllFeatureSliderAsync()
        {
            var responseMessage = await _httpClient.GetAsync("featuresliders");
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultFeatureSliderDto>>(jsonData);
            return values;
        }
        public async Task UpdateFeatureSliderAsync(UpdateFeatureSliderDto updateFeatureSliderDto)
        {
            await _httpClient.PutAsJsonAsync<UpdateFeatureSliderDto>("featuresliders", updateFeatureSliderDto);
        }

        public async Task FeatureSliderChangeStatusToTrue(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            // URL'yi API ile uyumlu hale getiriyoruz
            await _httpClient.PutAsync($"featuresliders/ChangeStatusToTrue?id={id}", null);
        }

        public async Task FeatureSliderChangeStatusToFalse(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            // URL'yi API ile uyumlu hale getiriyoruz
            await _httpClient.PutAsync($"featuresliders/ChangeStatusToFalse?id={id}", null);
        }

    }
}
