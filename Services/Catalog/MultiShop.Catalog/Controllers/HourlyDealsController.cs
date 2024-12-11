using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Dtos.HourlyDealDtos;
using MultiShop.Catalog.Services.HourlyDealServices;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HourlyDealsController : ControllerBase
    {
        private readonly IHourlyDealService _hourlyDealService;

        public HourlyDealsController(IHourlyDealService hourlyDealService)
        {
            _hourlyDealService = hourlyDealService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateHourlyDeals()
        {
            var deals = await _hourlyDealService.GenerateHourlyDealsAsync();
            return Ok(deals);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentHourlyDeals()
        {
            var currentDeals = await _hourlyDealService.GetCurrentHourlyDealsAsync();
            return Ok(currentDeals);
        }

        [HttpGet("endtimes")]
        public async Task<IActionResult> GetHourlyDealEndTimes()
        {
            try
            {
                var endTimes = await _hourlyDealService.GetHourlyDealEndTimesAsync();
                return Ok(endTimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}