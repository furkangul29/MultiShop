using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.HourlyDealDtos;
using MultiShop.Catalog.Entites;

namespace MultiShop.Catalog.Services.HourlyDealServices
{
    public class HourlyDealService : IHourlyDealService
    {
        private readonly IMongoCollection<HourlyDeal> _hourlyDealCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<DealsOfDay> _dealsOfDayCollection;
        private readonly IMapper _mapper;

        public HourlyDealService(
            IMapper mapper,
            IMongoCollection<HourlyDeal> hourlyDealCollection,
            IMongoCollection<Product> productCollection,
            IMongoCollection<Category> categoryCollection,
            IMongoCollection<DealsOfDay> dealsOfDayCollection)
        {
            _mapper = mapper;
            _hourlyDealCollection = hourlyDealCollection;
            _productCollection = productCollection;
            _categoryCollection = categoryCollection;
            _dealsOfDayCollection = dealsOfDayCollection;
        }

        public async Task<List<ResultHourlyDealDto>> GenerateHourlyDealsAsync()
        {
            try
            {
                // Mevcut saatte daha önce seçilmiş ürünleri al
                var currentHourDeals = await _hourlyDealCollection
                    .Find(x => x.StartTime.Date == DateTime.UtcNow.Date)
                    .ToListAsync();

                // Daha önce seçilmiş ürünlerin ID'lerini al
                var excludedProductIds = currentHourDeals
                    .Select(x => x.ProductId)
                    .ToList();

                // Aktif olan Daily Deals'ları da çıkar
                var activeDailyDeals = await _dealsOfDayCollection
                    .Find(x => x.IsActive)
                    .ToListAsync();
                excludedProductIds.AddRange(activeDailyDeals.Select(x => x.ProductId));

                // Uygun ürünleri filtrele
                var availableProducts = await _productCollection
                    .Find(x => !excludedProductIds.Contains(x.ProductId))
                    .ToListAsync();

                if (availableProducts.Count == 0)
                {
                    return new List<ResultHourlyDealDto>();
                }

                // Rastgele 5 ürün seç (veya mevcut ürün sayısı kadar)
                var random = new Random();
                var selectedProducts = availableProducts
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(5, availableProducts.Count))
                    .ToList();

                // Seçilen ürünlerin kategorilerini al
                var categoryIds = selectedProducts
                    .Where(p => !string.IsNullOrEmpty(p.CategoryId))
                    .Select(x => x.CategoryId)
                    .Distinct()
                    .ToList();

                var categories = categoryIds.Any()
                    ? await _categoryCollection
                        .Find(x => categoryIds.Contains(x.CategoryId))
                        .ToListAsync()
                    : new List<Category>();

                // Hourly Deal'ları oluştur
                var hourlyDeals = new List<HourlyDeal>();
                var resultDeals = new List<ResultHourlyDealDto>();

                foreach (var product in selectedProducts)
                {
                    var deal = new HourlyDeal
                    {
                        ProductId = product.ProductId,
                        OriginalPrice = product.ProductPrice,
                        DiscountPercentage = 25,
                        DiscountedPrice = product.ProductPrice * 0.75m,
                        StartTime = DateTime.UtcNow,
                        EndTime = DateTime.UtcNow.AddHours(3)
                    };

                    // Ürün fiyatını güncelle
                    var productUpdate = Builders<Product>.Update
                        .Set(p => p.ProductPrice, deal.DiscountedPrice);
                    await _productCollection.UpdateOneAsync(
                        p => p.ProductId == product.ProductId,
                        productUpdate);

                    // Deal'ı kaydet
                    await _hourlyDealCollection.InsertOneAsync(deal);

                    // Kategoriyi bul
                    var category = categories
                        .FirstOrDefault(c => c.CategoryId == product.CategoryId);

                    // DTO oluştur
                    var resultDeal = new ResultHourlyDealDto
                    {
                        HourlyDealId = deal.HourlyDealId,
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductImageUrl = product.ProductImageUrl,
                        OriginalPrice = deal.OriginalPrice,
                        DiscountedPrice = deal.DiscountedPrice,
                        DiscountPercentage = deal.DiscountPercentage,
                        StartTime = deal.StartTime,
                        EndTime = deal.EndTime,
                        CategoryName = category?.CategoryName ?? "Kategori Bulunamadı"
                    };

                    resultDeals.Add(resultDeal);
                }

                return resultDeals;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                throw new Exception("Saatlik İndirimler oluşturulurken bir hata oluştu.", ex);
            }
        }

        public async Task<List<ResultHourlyDealDto>> GetCurrentHourlyDealsAsync()
        {
            var currentTime = DateTime.UtcNow;
            var currentDeals = await _hourlyDealCollection
                .Find(x => x.StartTime <= currentTime && x.EndTime >= currentTime)
                .ToListAsync();

            var productIds = currentDeals.Select(x => x.ProductId).ToList();

            var products = await _productCollection
                .Find(x => productIds.Contains(x.ProductId))
                .ToListAsync();

            var categoryIds = products
                .Where(p => !string.IsNullOrEmpty(p.CategoryId))
                .Select(x => x.CategoryId)
                .Distinct()
                .ToList();

            var categories = categoryIds.Any()
                ? await _categoryCollection
                    .Find(x => categoryIds.Contains(x.CategoryId))
                    .ToListAsync()
                : new List<Category>();

            return currentDeals.Select(deal =>
            {
                var product = products.FirstOrDefault(p => p.ProductId == deal.ProductId);
                var category = product != null && !string.IsNullOrEmpty(product.CategoryId)
                    ? categories.FirstOrDefault(c => c.CategoryId == product.CategoryId)
                    : null;

                return new ResultHourlyDealDto
                {
                    HourlyDealId = deal.HourlyDealId,
                    ProductId = deal.ProductId,
                    ProductName = product?.ProductName ?? "Ürün Bulunamadı",
                    ProductImageUrl = product?.ProductImageUrl ?? "/images/no-image.png",
                    OriginalPrice = deal.OriginalPrice,
                    DiscountedPrice = deal.DiscountedPrice,
                    DiscountPercentage = deal.DiscountPercentage,
                    StartTime = deal.StartTime,
                    EndTime = deal.EndTime,
                    CategoryName = category?.CategoryName ?? "Kategori Bulunamadı"
                };
            }).ToList();
        }

        public async Task DeactivateExpiredHourlyDealsAsync()
        {
            try
            {
                var currentTime = DateTime.UtcNow;
                var expiredDeals = await _hourlyDealCollection
                    .Find(x => x.EndTime <= currentTime)
                    .ToListAsync();

                foreach (var deal in expiredDeals)
                {
                    // Ürün fiyatını eski haline getir
                    var productUpdate = Builders<Product>.Update
                        .Set(p => p.ProductPrice, deal.OriginalPrice);

                    await _productCollection.UpdateOneAsync(
                        p => p.ProductId == deal.ProductId,
                        productUpdate);

                    // Expired deal'ı sil
                    await _hourlyDealCollection.DeleteOneAsync(
                        x => x.HourlyDealId == deal.HourlyDealId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Saatlik İndirimler temizlenirken bir hata oluştu.", ex);
            }
        }
    }
}