
using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;

namespace MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices
{
    public interface  IDealsOfDayService
    {
        Task CreateDealAsync(CreateDealsOfDayDto createDealDto);
        Task<bool> DeleteDealsOfDayAsync(string id);
        Task<ResultDealsOfDayDto> GetByIdDealAsync(string id);
        Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync();
        Task UpdateDealAsync(UpdateDealsOfDayDto updateDealDto);
        Task ChangeDealStatusAsync(string dealId, bool isActive);
        Task<ResultDealsOfDayDto> UpdateGetByIdDealAsync(string id);
    }
}
