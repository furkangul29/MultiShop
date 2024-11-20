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
        private readonly IMapper _mapper;

        public DealsOfDayService(IMapper mapper, IMongoCollection<Product> productCollection, IMongoCollection<DealsOfDay> dealsCollection)
        {
            _mapper = mapper;
            _productCollection = productCollection;
            _dealsCollection = dealsCollection;
        }

        public async Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync()
        {
            var deals = await _dealsCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultDealsOfDayDto>>(deals);
        }

        public async Task CreateDealsOfDayAsync(CreateDealsOfDayDto createDealDto)
        {
            try
            {
                var deal = _mapper.Map<DealsOfDay>(createDealDto);
                deal.StartDate = DateTime.UtcNow;
                deal.EndDate = deal.StartDate.AddDays(1);
                deal.IsActive = true;
                deal.OriginalPrice = createDealDto.OriginalPrice;
                deal.DiscountedPrice = deal.OriginalPrice * (1 - (decimal)deal.DiscountPercentage / 100);

                await _dealsCollection.InsertOneAsync(deal);

                var update = Builders<Product>.Update
                    .Set(p => p.ProductPrice, deal.DiscountedPrice);

                await _productCollection.UpdateOneAsync(
                    p => p.ProductId == createDealDto.ProductId,
                    update);
            }
            catch (Exception ex)
            {
                // Burada hata yönetimi yapabilirsiniz
                throw;
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

        public async Task DeleteDealsOfDayAsync(string id)
        {
            try
            {
                var deal = await _dealsCollection
                    .Find(x => x.DealsOfDayId == id)
                    .FirstOrDefaultAsync();

                if (deal != null)
                {
                    await _dealsCollection.DeleteOneAsync(x => x.DealsOfDayId == id);

                    var update = Builders<Product>.Update
                        .Set(p => p.ProductPrice, deal.OriginalPrice);

                    await _productCollection.UpdateOneAsync(
                        p => p.ProductId == deal.ProductId,
                        update);
                }
            }
            catch (Exception ex)
            {
                throw;
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