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

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateHourlyDeals()
        {
            var deals = await _hourlyDealService.GenerateHourlyDealsAsync();
            return Ok(deals);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentHourlyDeals()
        {
            var currentDeals = await _hourlyDealService.GetCurrentHourlyDealsAsync();
            return Ok(currentDeals);
        }
    }
}