using MultiShop.Catalog.Dtos.HourlyDealDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Services.HourlyDealServices
{
    public interface IHourlyDealService
    {
        Task<List<ResultHourlyDealDto>> GenerateHourlyDealsAsync();
        Task<List<ResultHourlyDealDto>> GetCurrentHourlyDealsAsync();
        Task DeactivateExpiredHourlyDealsAsync();
        Task<List<DateTime>> GetHourlyDealEndTimesAsync();
    }
}