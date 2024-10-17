using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.CategoryDtos;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Entites;
using MultiShop.Catalog.Settings;
using MultiShop.DtoLayer.CatalogDtos.ProductDtos;

namespace MultiShop.Catalog.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        public ProductService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _categoryCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);
            _mapper = mapper;
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

        public async Task<List<ResultProductsWithCategoryDto>> GetFilteredProductsAsync(ProductFilterDto filterDto)
        {
            // Dinamik filtreleme için MongoDB filtre sorguları oluşturuyoruz
            var filter = Builders<Product>.Filter.Empty;

            // Fiyat aralığı filtreleme
            if (filterDto.MinPrice.HasValue && filterDto.MaxPrice.HasValue)
            {
                filter &= Builders<Product>.Filter.Gte(x => x.ProductPrice, filterDto.MinPrice.Value)
                       & Builders<Product>.Filter.Lte(x => x.ProductPrice, filterDto.MaxPrice.Value);
            }

            // Renk filtreleme
            if (!string.IsNullOrEmpty(filterDto.Color))
            {
                filter &= Builders<Product>.Filter.Eq(x => x.Color, filterDto.Color);
            }

            // Boyut filtreleme
            if (!string.IsNullOrEmpty(filterDto.Size))
            {
                filter &= Builders<Product>.Filter.Eq(x => x.Size, filterDto.Size);
            }

            // Kategori ID filtreleme
            if (!string.IsNullOrEmpty(filterDto.CategoryId))
            {
                filter &= Builders<Product>.Filter.Eq(x => x.CategoryId, filterDto.CategoryId);
            }

            // Filtrelenmiş ürünleri sorguluyoruz
            var products = await _productCollection.Find(filter).ToListAsync();

            // Ürünlere ait kategorileri sorguluyoruz
            foreach (var item in products)
            {
                item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstOrDefaultAsync();
            }

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
            var values = await _productCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultProductDto>>(values);
        }
        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var values = _mapper.Map<Product>(updateProductDto);
            await _productCollection.FindOneAndReplaceAsync(x => x.ProductId == updateProductDto.ProductId, values);
        }
    }
}
