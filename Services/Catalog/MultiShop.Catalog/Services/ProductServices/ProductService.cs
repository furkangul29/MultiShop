using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.CategoryDtos;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Entites;
using MultiShop.Catalog.Settings;
using MultiShop.DtoLayer.CatalogDtos.ProductDtos;
using Newtonsoft.Json;

namespace MultiShop.Catalog.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IMapper mapper, IDatabaseSettings _databaseSettings, IHttpClientFactory httpClientFactory)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _categoryCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }
        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var values = _mapper.Map<Product>(createProductDto);
            await _productCollection.InsertOneAsync(values);
        }
        public async Task DeleteProductAsync(string id)
        {
            await _productCollection.DeleteOneAsync(x => x.ProductId == id);
        }
        public async Task<GetByIdProductDto> GetByIdProductAsync(string id)
        {
            var values = await _productCollection.Find<Product>(x => x.ProductId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdProductDto>(values);
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetFilteredProductsAsync(ProductFiltersDto filterDto)
        {
            // Başlangıç filtresi boş olarak tanımlanıyor.
            var filter = Builders<Product>.Filter.Empty;

            // Debug bilgileri için bir liste
            var debugInfo = new List<string>();

            // Kategori ID filtreleme
            if (!string.IsNullOrEmpty(filterDto.CategoryId))
            {
                filter &= Builders<Product>.Filter.Eq(x => x.CategoryId, filterDto.CategoryId);
                debugInfo.Add($"Category filter applied: {filterDto.CategoryId}");
            }

            // Fiyat aralığı filtreleme (birden fazla fiyat aralığı seçilebilir)
            if (filterDto.SelectedPrices != null && filterDto.SelectedPrices.Any())
            {
                var priceFilters = new List<FilterDefinition<Product>>();

                foreach (var priceRange in filterDto.SelectedPrices)
                {
                    if (priceRange.MinPrice.HasValue && priceRange.MaxPrice.HasValue)
                    {
                        priceFilters.Add(Builders<Product>.Filter.And(
                            Builders<Product>.Filter.Gte(x => x.ProductPrice, priceRange.MinPrice.Value),
                            Builders<Product>.Filter.Lte(x => x.ProductPrice, priceRange.MaxPrice.Value)
                        ));
                        debugInfo.Add($"Price range filter applied: {priceRange.MinPrice.Value} - {priceRange.MaxPrice.Value}");
                    }
                }

                // Fiyat aralığı filtrelerini OR ile birleştiriyoruz (çoklu fiyat aralığı)
                if (priceFilters.Count > 0)
                {
                    filter &= Builders<Product>.Filter.Or(priceFilters);
                }
            }

            // Renk filtreleme
            if (filterDto.SelectedColors != null && filterDto.SelectedColors.Any())
            {
                filter &= Builders<Product>.Filter.In(x => x.Color, filterDto.SelectedColors);
                debugInfo.Add($"Color filter applied: {string.Join(", ", filterDto.SelectedColors)}");
            }

            // Beden filtreleme
            if (filterDto.SelectedSizes != null && filterDto.SelectedSizes.Any())
            {
                filter &= Builders<Product>.Filter.In(x => x.Size, filterDto.SelectedSizes);
                debugInfo.Add($"Size filter applied: {string.Join(", ", filterDto.SelectedSizes)}");
            }

            // Tüm filtreler tamamlandıktan sonra veritabanından ürünler çekiliyor
            var products = await _productCollection.Find(filter).ToListAsync();
            debugInfo.Add($"Total filtered products: {products.Count}");

            // Ürün bilgilerini detaylı loglamak
            foreach (var product in products)
            {
                debugInfo.Add($"Product: {product.ProductName}, Price: {product.ProductPrice}, CategoryId: {product.CategoryId}, Color: {product.Color}, Size: {product.Size}");
            }

            // Debug bilgilerini loglama (isteğe bağlı olarak)
            //_logger.LogInformation("Filter Debug Info: " + string.Join("\n", debugInfo));

            // DTO'ya dönüştürüp sonuçları döndürüyoruz
            return _mapper.Map<List<ResultProductsWithCategoryDto>>(products);
        }



        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryAsync()
        {
            var values = await _productCollection.Find(x => true).ToListAsync();

            foreach (var item in values)
            {
                item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstAsync();
            }

            return _mapper.Map<List<ResultProductsWithCategoryDto>>(values);
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryByCatetegoryIdAsync(string CategoryId)
        {
            var values = await _productCollection.Find(x => x.CategoryId == CategoryId).ToListAsync();

            foreach (var item in values)
            {
                item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstAsync();
            }

            return _mapper.Map<List<ResultProductsWithCategoryDto>>(values);
        }

        public async Task<List<ResultProductDto>> GettAllProductAsync()
        {
            var products = await _productCollection.Find(x => true).ToListAsync();
            var mappedProducts = _mapper.Map<List<ResultProductDto>>(products);

            var httpClient = _httpClientFactory.CreateClient("CommentAPI");

            foreach (var product in mappedProducts)
            {
                var requestUrl = $"api/comments/GetProductRatingStats?productId={product.ProductId}"; // Parametreli URL

                // Header eklemek
                httpClient.DefaultRequestHeaders.Clear(); // Varsayılan header'ları temizleyin
                httpClient.DefaultRequestHeaders.Add("accept", "*/*");

                var responseMessage = await httpClient.GetAsync(requestUrl);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var ratingStats = JsonConvert.DeserializeObject<ProductRatingStatsDto>(jsonData);

                    product.AverageRating = ratingStats.AverageRating;
                    product.ReviewCount = ratingStats.TotalReviews;
                }
                else
                {
                    // Hata durumunda yapılacak işlemler
                    // Örneğin, loglama veya varsayılan değerler
                }
            }

            return mappedProducts;
        }


        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var values = _mapper.Map<Product>(updateProductDto);
            await _productCollection.FindOneAndReplaceAsync(x => x.ProductId == updateProductDto.ProductId, values);
        }

        public async Task<List<ResultProductDto>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 2)
                return new List<ResultProductDto>();

            var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(x => x.ProductName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex(x => x.ProductDescription, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );

            var products = await _productCollection
                .Find(filter)
                .Limit(10) 
                .ToListAsync();

            return _mapper.Map<List<ResultProductDto>>(products);
        }
    }
}
