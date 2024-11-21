using MultiShop.Catalog.Dtos.DealsOfDayDtos;

namespace MultiShop.Catalog.Services.DealsOfDayServices
{
    public interface IDealsOfDayService
    {
        Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync();
        Task CreateDealsOfDayAsync(CreateDealsOfDayDto createDealDto);
        Task UpdateDealsOfDayAsync(UpdateDealsOfDayDto updateDealDto);
        Task<bool> DeleteDealsOfDayAsync(string id);
        Task<ResultDealsOfDayDto> GetDealsOfDayByIdAsync(string id);
        Task DeactivateExpiredDealsAsync();
        Task ChangeDealStatusAsync(string dealId, bool isActive);
        Task<ResultDealsOfDayDto> UpdateGetByIdDealAsync(string dealId);
    }
}
