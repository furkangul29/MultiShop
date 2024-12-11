using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.CatalogServices.HourlyDealServices;

public class HourlyDealController : Controller
{
    private readonly IHourlyDealService _hourlyDealService;

    public HourlyDealController(IHourlyDealService hourlyDealService)
    {
        _hourlyDealService = hourlyDealService;
    }

    public async Task<IActionResult> Index()
    {
        var currentDeals = await _hourlyDealService.GetCurrentHourlyDealsAsync();
        var endTimes = currentDeals
            .Select(deal => deal.EndTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss")) // ISO formatında, "Z" eklemiyoruz.
            .FirstOrDefault(); // İlk fırsatın bitiş zamanını al

        ViewBag.EndTimes = endTimes; // ViewBag ile gönder

        return View(currentDeals);
    }


    [HttpGet] 
    public async Task<IActionResult> GetEndTimes()
    {
        var endTimes = await _hourlyDealService.GetHourlyDealEndTimesAsync();
        return Ok(endTimes);
    }
}
