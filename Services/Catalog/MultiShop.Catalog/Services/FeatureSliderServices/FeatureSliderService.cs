using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.CategoryDtos;
using MultiShop.Catalog.Dtos.FeatureSliderDtos;
using MultiShop.Catalog.Entites;
using MultiShop.Catalog.Settings;

namespace MultiShop.Catalog.Services.FeatureSliderServices
{
    public class FeatureSliderService : IFeatureSliderService
    {
        private readonly IMongoCollection<FeatureSlider> _featureSliderCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public FeatureSliderService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _featureSliderCollection = database.GetCollection<FeatureSlider>(databaseSettings.FeatureSliderCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task CreateFeatureSliderAsync(CreateFeatureSliderDto createFeatureSliderDto)
        {
            var category = await _categoryCollection.Find(x => x.CategoryId == createFeatureSliderDto.CategoryId).FirstOrDefaultAsync();
      

            var value = _mapper.Map<FeatureSlider>(createFeatureSliderDto);
            await _featureSliderCollection.InsertOneAsync(value);
        }

        public async Task DeleteFeatureSliderAsync(string id)
        {
            await _featureSliderCollection.DeleteOneAsync(x => x.FeatureSliderId == id);
        }


        public async Task FeatureSliderChangeStatusToFalse(string id)
        {
            var filter = Builders<FeatureSlider>.Filter.Eq(x => x.FeatureSliderId, id);
            var update = Builders<FeatureSlider>.Update.Set(x => x.Status, false);
            await _featureSliderCollection.UpdateOneAsync(filter, update);
        }

        public async Task FeatureSliderChangeStatusToTrue(string id)
        {
            var filter = Builders<FeatureSlider>.Filter.Eq(x => x.FeatureSliderId, id);
            var update = Builders<FeatureSlider>.Update.Set(x => x.Status, true);
            await _featureSliderCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<ResultFeatureSliderDto>> GetAllFeatureSliderAsync()
        {
            var values = await _featureSliderCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultFeatureSliderDto>>(values);
        }

        public async Task<GetByIdFeatureSliderDto> GetByIdFeatureSliderAsync(string id)
        {
            var values = await _featureSliderCollection.Find<FeatureSlider>(x => x.FeatureSliderId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdFeatureSliderDto>(values);
        }

        public async Task UpdateFeatureSliderAsync(UpdateFeatureSliderDto updateFeatureSliderDto)
        {
            var category = await _categoryCollection.Find(x => x.CategoryId == updateFeatureSliderDto.CategoryId).FirstOrDefaultAsync();
         

            var values = _mapper.Map<FeatureSlider>(updateFeatureSliderDto);
            await _featureSliderCollection.FindOneAndReplaceAsync(x => x.FeatureSliderId == updateFeatureSliderDto.FeatureSliderId, values);
        }
    }
}