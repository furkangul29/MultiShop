using MongoDB.Bson;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Entites;
using MultiShop.Catalog.Services.ProductServices;

namespace MultiShop.Catalog.Services.FavoriteProductServices
{
    public class FavoriteProductService : IFavoriteProductService
    {
        private readonly IMongoCollection<FavoriteProduct> _favoriteCollection;
        private readonly IProductService _productService;

        public FavoriteProductService(
            IMongoDatabase database,
            IProductService productService)
        {
            _favoriteCollection = database.GetCollection<FavoriteProduct>("FavoriteProducts");
            _productService = productService;
        }

        public async Task<bool> AddToFavorites(string productId, string userId)
        {
            var exists = await _favoriteCollection
                .Find(x => x.ProductId == productId && x.UserId == userId)
                .AnyAsync();

            if (exists)
                return false;

            var favoriteProduct = new FavoriteProduct
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ProductId = productId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _favoriteCollection.InsertOneAsync(favoriteProduct);
            return true;
        }

        public async Task<bool> RemoveFromFavorites(string productId, string userId)
        {
            var result = await _favoriteCollection.DeleteOneAsync(
                x => x.ProductId == productId && x.UserId == userId);

            return result.DeletedCount > 0;
        }

        public async Task<List<FavoriteProductDto>> GetUserFavorites(string userId)
        {
            var favorites = await _favoriteCollection
                .Find(x => x.UserId == userId)
                .ToListAsync();

            var favoriteProducts = new List<FavoriteProductDto>();

            foreach (var favorite in favorites)
            {
                var product = await _productService.GetByIdProductAsync(favorite.ProductId);
                if (product != null)
                {
                    favoriteProducts.Add(new FavoriteProductDto
                    {
                        ProductId = favorite.ProductId,
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice,
                        ProductImageUrl = product.ProductImageUrl,
                        AddedDate = favorite.CreatedDate
                    });
                }
            }

            return favoriteProducts;
        }

        public async Task<int> GetFavoriteCount(string userId)
        {
            return (int)await _favoriteCollection
                .CountDocumentsAsync(x => x.UserId == userId);
        }

        public async Task<bool> IsProductFavorited(string productId, string userId)
        {
            return await _favoriteCollection
                .Find(x => x.ProductId == productId && x.UserId == userId)
                .AnyAsync();
        }
    }
}
