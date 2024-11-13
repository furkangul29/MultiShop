
using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;

namespace MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices
{
    public interface  IDealsOfDayService
    {
        Task CreateDealAsync(CreateDealsOfDayDto createDealDto);
        Task DeleteDealAsync(string id);
        Task<UpdateDealsOfDayDto> GetByIdDealAsync(string id);
        Task<List<ResultDealsOfDayDto>> GetAllDealsOfDayAsync();
        Task UpdateDealAsync(UpdateDealsOfDayDto updateDealDto);

    }
}
