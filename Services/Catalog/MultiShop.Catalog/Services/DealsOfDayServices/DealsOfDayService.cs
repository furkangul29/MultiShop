using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.DealsOfDayDtos;
using MultiShop.Catalog.Entites;
using MultiShop.Catalog.Settings;

namespace MultiShop.Catalog.Services.DealsOfDayServices
{
    public class DealsOfDayService : IDealsOfDayService
    {
        private readonly IMongoCollection<DealsOfDay> _dealsCollection;
        private readonly IMapper _mapper;

        public DealsOfDayService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _dealsCollection = database.GetCollection<DealsOfDay>(databaseSettings.DealsOfDayCollectionName);
            _mapper = mapper;
        }

        public async Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync()
        {
            var deals = await _dealsCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultDealsOfDayDto>>(deals);
        }

        public async Task CreateDealsOfDayAsync(CreateDealsOfDayDto createDealDto)
        {
            var deal = _mapper.Map<DealsOfDay>(createDealDto);
            await _dealsCollection.InsertOneAsync(deal);
        }

        public async Task UpdateDealsOfDayAsync(UpdateDealsOfDayDto updateDealDto)
        {
            var updatedDeal = _mapper.Map<DealsOfDay>(updateDealDto);
            await _dealsCollection.ReplaceOneAsync(x => x.DealsOfDayId == updateDealDto.DealsOfDayId, updatedDeal);
        }

        public async Task DeleteDealsOfDayAsync(string id)
        {
            await _dealsCollection.DeleteOneAsync(x => x.DealsOfDayId == id);
        }

        public async Task<ResultDealsOfDayDto> GetDealsOfDayByIdAsync(string id)
        {
            var deal = await _dealsCollection.Find(x => x.DealsOfDayId == id).FirstOrDefaultAsync();
            return _mapper.Map<ResultDealsOfDayDto>(deal);
        }
    }
}
