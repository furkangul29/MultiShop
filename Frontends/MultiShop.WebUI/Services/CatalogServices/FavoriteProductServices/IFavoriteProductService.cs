

using MultiShop.DtoLayer.CatalogDtos.ProductDtos;

namespace MultiShop.WebUI.Services.CatalogServices.FavoriteProductServices
{
    public interface IFavoriteProductService
    {
        Task<bool> AddToFavorites(string productId, string userId);
        Task<bool> RemoveFromFavorites(string productId, string userId);
        Task<List<FavoriteProductDto>> GetUserFavorites(string userId);
        Task<int> GetFavoriteCount(string userId);
        Task<bool> IsProductFavorited(string productId, string userId);
    }
}
