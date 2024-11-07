using MultiShop.DtoLayer.CatalogDtos.ProductDtos;
using System.Net.Http.Json;

namespace MultiShop.WebUI.Services.CatalogServices.FavoriteProductServices
{
    public class FavoriteProductService : IFavoriteProductService
    {
        private readonly HttpClient _httpClient;

        public FavoriteProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddToFavorites(string productId, string userId)
        {
            try
            {
                var request = new { ProductId = productId, UserId = userId };
                var response = await _httpClient.PostAsJsonAsync("favoriteproducts", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<FavoriteProductDto>> GetUserFavorites(string userId)
        {
            var response = await _httpClient.GetAsync($"favoriteproducts/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var favorites = await response.Content.ReadFromJsonAsync<List<FavoriteProductDto>>();
                return favorites ?? new List<FavoriteProductDto>();
            }
            return new List<FavoriteProductDto>();
        }

        public async Task<int> GetFavoriteCount(string userId)
        {
            var response = await _httpClient.GetAsync($"favoriteproducts/count/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var count = await response.Content.ReadFromJsonAsync<int>();
                return count;
            }
            return 0;
        }

        public async Task<bool> IsProductFavorited(string productId, string userId)
        {
            var response = await _httpClient.GetAsync($"favoriteproducts/check/{productId}/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        public async Task<bool> RemoveFromFavorites(string productId, string userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"favoriteproducts/{productId}/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}