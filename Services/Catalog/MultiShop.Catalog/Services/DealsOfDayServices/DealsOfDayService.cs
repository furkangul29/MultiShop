using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.DealsOfDayDtos;
using MultiShop.Catalog.Entites;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Services.DealsOfDayServices
{
    public class DealsOfDayService : IDealsOfDayService
    {
        private readonly IMongoCollection<DealsOfDay> _dealsCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public DealsOfDayService(IMapper mapper, IMongoCollection<Product> productCollection, IMongoCollection<DealsOfDay> dealsCollection, IMongoCollection<Category> categoryCollection)
        {
            _mapper = mapper;
            _productCollection = productCollection;
            _dealsCollection = dealsCollection;
            _categoryCollection = categoryCollection;
        }

        public async Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync()
        {
            try
            {
                // Önce tüm deals'leri al
                var deals = await _dealsCollection
                    .Find(_ => true)
                    .ToListAsync();

                if (!deals.Any())
                    return new List<ResultDealsOfDayDto>();

                // Product ID'lerini topla
                var productIds = deals.Select(x => x.ProductId).ToList();

                // Products'ları getir
                var products = await _productCollection
                    .Find(x => productIds.Contains(x.ProductId))
                    .ToListAsync();

                if (!products.Any())
                    return _mapper.Map<List<ResultDealsOfDayDto>>(deals);

                // Category ID'lerini topla
                var categoryIds = products
                    .Where(p => !string.IsNullOrEmpty(p.CategoryId))
                    .Select(x => x.CategoryId)
                    .Distinct()
                    .ToList();

                // Categories'leri getir
                var categories = new List<Category>();
                if (categoryIds.Any())
                {
                    categories = await _categoryCollection
                        .Find(x => categoryIds.Contains(x.CategoryId))
                        .ToListAsync();
                }

                // DTO'ları oluştur
                var resultList = deals.Select(deal =>
                {
                    var product = products.FirstOrDefault(p => p.ProductId == deal.ProductId);
                    var category = product != null && !string.IsNullOrEmpty(product.CategoryId)
                        ? categories.FirstOrDefault(c => c.CategoryId == product.CategoryId)
                        : null;

                    return new ResultDealsOfDayDto
                    {
                        DealsOfDayId = deal.DealsOfDayId,
                        ProductId = deal.ProductId,
                        ProductName = product?.ProductName ?? "Ürün Bulunamadı",
                        ProductImageUrl = product?.ProductImageUrl ?? "/images/no-image.png",
                        DiscountPercentage = deal.DiscountPercentage,
                        OriginalPrice = deal.OriginalPrice,
                        DiscountedPrice = deal.DiscountedPrice,
                        IsActive = deal.IsActive,
                        CategoryName = category?.CategoryName ?? "Kategori Bulunamadı",
                        StartDate = deal.StartDate,
                        EndDate = deal.EndDate
                    };
                }).ToList();

                return resultList;
            }
            catch (Exception ex)
            {
                // Loglama yapabilirsiniz
                throw;
            }
        }

        public async Task CreateDealsOfDayAsync(CreateDealsOfDayDto createDealDto)
        {
            try
            {
                // Aynı ürün için aktif bir kampanya var mı kontrol et
                var existingDeal = await _dealsCollection.Find(x =>
                    x.ProductId == createDealDto.ProductId &&
                    x.IsActive == true &&
                    x.EndDate > DateTime.UtcNow.AddHours(3)  // Türkiye saati için +3
                ).FirstOrDefaultAsync();
                if (existingDeal != null)
                {
                    // Kampanya zaten mevcut, Toast mesajı için özel bir exception fırlat
                    throw new Exception($"Bu ürün için zaten aktif bir kampanya bulunmaktadır. Kampanya bitiş tarihi: {existingDeal.EndDate.ToString("dd/MM/yyyy HH:mm")}");
                }

                // Yeni kampanyayı oluştur
                var deal = _mapper.Map<DealsOfDay>(createDealDto);
                deal.StartDate = DateTime.UtcNow.AddHours(3);  // Türkiye saati için +3
                deal.EndDate = deal.StartDate.AddDays(1);
                deal.IsActive = true;
                deal.OriginalPrice = createDealDto.OriginalPrice;
                deal.DiscountedPrice = deal.OriginalPrice * (1 - (decimal)deal.DiscountPercentage / 100);

                // Kampanyayı kaydet
                await _dealsCollection.InsertOneAsync(deal);

                // Ürün fiyatını güncelle
                var update = Builders<Product>.Update
                    .Set(p => p.ProductPrice, deal.DiscountedPrice);
                var updateResult = await _productCollection.UpdateOneAsync(
                    p => p.ProductId == createDealDto.ProductId,
                    update);

                if (updateResult.ModifiedCount == 0)
                {
                    // Ürün bulunamadı veya güncelleme başarısız
                    // Kampanyayı geri al
                    await _dealsCollection.DeleteOneAsync(x => x.DealsOfDayId == deal.DealsOfDayId);
                    throw new Exception("Ürün fiyatı güncellenirken bir hata oluştu. Kampanya oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını koruyarak yeniden fırlat
                throw new Exception($"Günün Fırsatı oluşturulurken bir hata oluştu: {ex.Message}", ex);
            }
        }


        public async Task UpdateDealsOfDayAsync(UpdateDealsOfDayDto updateDealDto)
        {
            try
            {
                var deal = _mapper.Map<DealsOfDay>(updateDealDto);

                var existingDeal = await _dealsCollection
                    .Find(x => x.DealsOfDayId == updateDealDto.DealsOfDayId)
                    .FirstOrDefaultAsync();

                if (existingDeal != null)
                {
                    deal.StartDate = existingDeal.StartDate;
                    deal.EndDate = existingDeal.EndDate;
                }

                deal.DiscountedPrice = deal.OriginalPrice * (1 - (decimal)deal.DiscountPercentage / 100);

                await _dealsCollection.ReplaceOneAsync(
                    x => x.DealsOfDayId == updateDealDto.DealsOfDayId,
                    deal);

                var update = Builders<Product>.Update
                    .Set(p => p.ProductPrice, deal.DiscountedPrice);

                await _productCollection.UpdateOneAsync(
                    p => p.ProductId == updateDealDto.ProductId,
                    update);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResultDealsOfDayDto> UpdateGetByIdDealAsync(string dealId)
        {
            try
            {
                // Belirtilen ID'ye sahip olan fırsatı getir
                var deal = await _dealsCollection
                    .Find(d => d.DealsOfDayId == dealId)
                    .FirstOrDefaultAsync();

                if (deal == null)
                    throw new Exception("Belirtilen ID'ye sahip fırsat bulunamadı.");

                // İlgili Product bilgilerini getir
                var product = await _productCollection
                    .Find(p => p.ProductId == deal.ProductId)
                    .FirstOrDefaultAsync();

                // İlgili Category bilgilerini getir
                Category? category = null;
                if (product != null && !string.IsNullOrEmpty(product.CategoryId))
                {
                    category = await _categoryCollection
                        .Find(c => c.CategoryId == product.CategoryId)
                        .FirstOrDefaultAsync();
                }

                // DTO'yu oluştur ve döndür
                var resultDealDto = new ResultDealsOfDayDto
                {
                    DealsOfDayId = deal.DealsOfDayId,
                    ProductId = deal.ProductId,
                    ProductName = product?.ProductName ?? "Ürün Bulunamadı",
                    ProductImageUrl = product?.ProductImageUrl ?? "/images/no-image.png",
                    DiscountPercentage = deal.DiscountPercentage,
                    OriginalPrice = deal.OriginalPrice,
                    DiscountedPrice = deal.DiscountedPrice,
                    IsActive = deal.IsActive,
                    CategoryName = category?.CategoryName ?? "Kategori Bulunamadı",
                    StartDate = deal.StartDate,
                    EndDate = deal.EndDate
                };

                return resultDealDto;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama veya hata mesajı oluşturabilirsiniz
                throw new Exception("Fırsat bilgisi alınırken bir hata oluştu.", ex);
            }
        }



        public async Task<bool> DeleteDealsOfDayAsync(string id)
        {
            try
            {
                // Fırsat verisini buluyoruz
                var deal = await _dealsCollection
                    .Find(x => x.DealsOfDayId == id)
                    .FirstOrDefaultAsync();

                if (deal == null)
                {
                    throw new Exception("Deal not found"); // Daha açıklayıcı bir hata mesajı
                }

                // Fırsat verisini siliyoruz
                var deleteResult = await _dealsCollection.DeleteOneAsync(x => x.DealsOfDayId == id);

                if (deleteResult.DeletedCount == 0)
                {
                    throw new Exception("Failed to delete deal from collection");
                }

                // Ürün fiyatını eski haline getiriyoruz
                var update = Builders<Product>.Update
                    .Set(p => p.ProductPrice, deal.OriginalPrice);

                // Ürün fiyatını güncelleyerek silme işlemi tamamlıyoruz
                var updateResult = await _productCollection.UpdateOneAsync(
                    p => p.ProductId == deal.ProductId,
                    update);

                if (updateResult.ModifiedCount == 0)
                {
                    throw new Exception("Failed to update product price");
                }

                return true; // Her şey başarılıysa
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }



        public async Task<ResultDealsOfDayDto> GetDealsOfDayByIdAsync(string id)
        {
            var deal = await _dealsCollection.Find(x => x.DealsOfDayId == id).FirstOrDefaultAsync();
            return _mapper.Map<ResultDealsOfDayDto>(deal);
        }

        public async Task DeactivateExpiredDealsAsync()
        {
            try
            {
                var currentTime = DateTime.UtcNow;
                var expiredDeals = await _dealsCollection
                    .Find(x => x.IsActive && x.EndDate <= currentTime)
                    .ToListAsync();

                foreach (var deal in expiredDeals)
                {
                    var dealUpdate = Builders<DealsOfDay>.Update
                        .Set(x => x.IsActive, false);

                    await _dealsCollection.UpdateOneAsync(
                        x => x.DealsOfDayId == deal.DealsOfDayId,
                        dealUpdate);

                    var productUpdate = Builders<Product>.Update
                        .Set(p => p.ProductPrice, deal.OriginalPrice);

                    await _productCollection.UpdateOneAsync(
                        p => p.ProductId == deal.ProductId,
                        productUpdate);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task ChangeDealStatusAsync(string dealId, bool isActive)
        {
            try
            {
                var existingDeal = await _dealsCollection.Find(x => x.DealsOfDayId == dealId).FirstOrDefaultAsync();
                if (existingDeal == null)
                    throw new Exception("Deal not found in the database.");

                if (existingDeal.IsActive == isActive)
                    return;

                // Önce deals durumunu güncelle
                var update = Builders<DealsOfDay>.Update.Set(x => x.IsActive, isActive);
                var result = await _dealsCollection.UpdateOneAsync(
                    x => x.DealsOfDayId == dealId,
                    update
                );

                // Şimdi ürün fiyatını güncelle
                var product = await _productCollection.Find(x => x.ProductId == existingDeal.ProductId).FirstOrDefaultAsync();
                if (product != null)
                {
                    var productUpdate = isActive
                        ? Builders<Product>.Update.Set(x => x.ProductPrice, existingDeal.DiscountedPrice)
                        : Builders<Product>.Update.Set(x => x.ProductPrice, existingDeal.OriginalPrice);

                    await _productCollection.UpdateOneAsync(
                        x => x.ProductId == existingDeal.ProductId,
                        productUpdate
                    );
                }

                if (result.ModifiedCount == 0)
                    throw new Exception("No changes were made to the deal.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to change deal status", ex);
            }
        }


    }

    public class DealsExpirationService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public DealsExpirationService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var dealsService = scope.ServiceProvider.GetRequiredService<IDealsOfDayService>();
                    await dealsService.DeactivateExpiredDealsAsync();
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}