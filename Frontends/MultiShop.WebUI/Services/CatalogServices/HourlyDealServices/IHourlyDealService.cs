using MultiShop.DtoLayer.CatalogDtos.HourlyDealDtos;

namespace MultiShop.WebUI.Services.CatalogServices.HourlyDealServices
{
    public interface IHourlyDealService
    {
        Task<List<ResultHourlyDealDto>> GetCurrentHourlyDealsAsync();
        Task<List<ResultHourlyDealDto>> GenerateHourlyDealsAsync();
        Task DeactivateExpiredHourlyDealsAsync();
        Task<List<DateTime>> GetHourlyDealEndTimesAsync();
    }
}
