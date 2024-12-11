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
                var currentTime = DateTime.UtcNow.AddHours(3);
                var currentHourDeals = await _hourlyDealCollection
                .Find(x => x.EndTime < currentTime).ToListAsync();

                var excludedProductIds = currentHourDeals
                    .Select(x => x.ProductId)
                    .ToList();

                var activeDailyDeals = await _dealsOfDayCollection
                    .Find(x => x.IsActive)
                    .ToListAsync();
                var activeDeals = await _hourlyDealCollection
                        .Find(x => x.StartTime <= currentTime && x.EndTime >= currentTime)
                        .ToListAsync();

                if (activeDeals.Any())
                {
                    throw new InvalidOperationException("Aktif saatlik indirimler mevcut, yeni indirimler oluşturulamaz.");
                }

                excludedProductIds.AddRange(activeDailyDeals.Select(x => x.ProductId));

                var availableProducts = await _productCollection
                    .Find(x => !excludedProductIds.Contains(x.ProductId))
                    .ToListAsync();

                if (availableProducts.Count == 0)
                {
                    return new List<ResultHourlyDealDto>();
                }

                var random = new Random();
                var selectedProducts = availableProducts
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(5, availableProducts.Count))
                    .ToList();

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
                        StartTime = DateTime.UtcNow.AddHours(3),
                        EndTime = DateTime.UtcNow.AddHours(6)
                    };

                    var productUpdate = Builders<Product>.Update
                        .Set(p => p.ProductPrice, deal.DiscountedPrice);
                    await _productCollection.UpdateOneAsync(
                        p => p.ProductId == product.ProductId,
                        productUpdate);

                    await _hourlyDealCollection.InsertOneAsync(deal);

                    var category = categories
                        .FirstOrDefault(c => c.CategoryId == product.CategoryId);

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
                throw new Exception("Saatlik İndirimler oluşturulurken bir hata oluştu.", ex);
            }
        }

        public async Task<List<ResultHourlyDealDto>> GetCurrentHourlyDealsAsync()
        {
            var currentTime = DateTime.UtcNow.AddHours(3);
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



        public async Task<List<DateTime>> GetHourlyDealEndTimesAsync()
        {
            var currentDeals = await GetCurrentHourlyDealsAsync();
            return currentDeals.Select(deal => deal.EndTime).ToList();
        }

        public async Task DeactivateExpiredHourlyDealsAsync()
        {
            try
            {
                var currentTime = DateTime.UtcNow.AddHours(3);
                var expiredDeals = await _hourlyDealCollection
                    .Find(x => x.EndTime <= currentTime)
                    .ToListAsync();

                foreach (var deal in expiredDeals)
                {
                    var productUpdate = Builders<Product>.Update
                        .Set(p => p.ProductPrice, deal.OriginalPrice);

                    await _productCollection.UpdateOneAsync(
                        p => p.ProductId == deal.ProductId,
                        productUpdate);

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
