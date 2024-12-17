using MultiShop.DtoLayer.BasketDtos;

namespace MultiShop.WebUI.Services.BasketServices
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        public BasketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task AddBasketItem(BasketItemDto basketItemDto)
        {
            var values = await GetBasket();

            if (values == null)
            {
                // Eğer sepet boşsa, yeni bir sepet oluştur.
                values = new BasketTotalDto
                {
                    BasketItems = new List<BasketItemDto>()
                };
            }

            if (!values.BasketItems.Any(x => x.ProductId == basketItemDto.ProductId))
            {
                // Aynı üründen yoksa, ürünü sepete ekle.
                values.BasketItems.Add(basketItemDto);
            }
            else
            {
                // Eğer ürün zaten sepette varsa, miktarını artır veya başka bir işlem yap.
                var existingItem = values.BasketItems.First(x => x.ProductId == basketItemDto.ProductId);
                existingItem.Quantity += basketItemDto.Quantity;
            }

            await SaveBasket(values);
        }


        public Task DeleteBasket(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<BasketTotalDto> GetBasket()
        {
            var responseMessage = await _httpClient.GetAsync("baskets");
            var values = await responseMessage.Content.ReadFromJsonAsync<BasketTotalDto>();
            return values;
        }

        public async Task<bool> RemoveBasketItem(string productId)
        {
            var values = await GetBasket();
            var deletedItem = values.BasketItems.FirstOrDefault(x => x.ProductId == productId);
            var result=values.BasketItems.Remove(deletedItem);
            await SaveBasket(values);
            return true;
        }

        public async Task SaveBasket(BasketTotalDto basketTotalDto)
        {
            await _httpClient.PostAsJsonAsync<BasketTotalDto>("baskets", basketTotalDto);
        }

        public async Task<int> GetBasketItemCount()
        {
            var responseMessage = await _httpClient.GetAsync("baskets/GetBasketItemCount");

            if (responseMessage.IsSuccessStatusCode)
            {
                // Doğru sınıfı kullanarak gelen yanıtı alıyoruz
                var values = await responseMessage.Content.ReadFromJsonAsync<ItemCountResponse>();
                return values?.ItemCount ?? 0;  // Eğer ItemCount null ise, 0 döndür
            }

            // Hata durumunda 0 döndür
            return 0;
        }

    }
}
