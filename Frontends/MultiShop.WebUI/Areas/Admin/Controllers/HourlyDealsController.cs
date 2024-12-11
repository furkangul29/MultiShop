using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.HourlyDealDtos;
using MultiShop.WebUI.Services.CatalogServices.HourlyDealServices;
using System;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/HourlyDeals")]
    public class HourlyDealsController : Controller
    {
        private readonly IHourlyDealService _hourlyDealService;
        private readonly ILogger<HourlyDealsController> _logger;

        public HourlyDealsController(
            IHourlyDealService hourlyDealService,
            ILogger<HourlyDealsController> logger)
        {
            _hourlyDealService = hourlyDealService;
            _logger = logger;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentDeals = await _hourlyDealService.GetCurrentHourlyDealsAsync();
                return View(currentDeals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Saatlik fırsatlar getirilirken hata oluştu");
                return View(new List<ResultHourlyDealDto>());
            }
        }

        [HttpPost]
        [Route("GenerateHourlyDeals")]

        public async Task<IActionResult> GenerateHourlyDeals()
        {
            try
            {
                var deals = await _hourlyDealService.GenerateHourlyDealsAsync();

                if (deals.Any())
                {
                    return Json(new
                    {
                        success = true,
                        message = $"{deals.Count} adet saatlik fırsat başarıyla oluşturuldu."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Saatlik fırsat oluşturmak için uygun ürün bulunamadı."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Saatlik fırsatlar oluşturulurken hata oluştu");
                return Json(new
                {
                    success = false,
                    message = "Saatlik fırsatlar oluşturulurken bir hata oluştu."
                });
            }
        }

        [HttpPost]
        [Route("ClearExpiredHourlyDeals")]

        public async Task<IActionResult> ClearExpiredHourlyDeals()
        {
            try
            {
                await _hourlyDealService.DeactivateExpiredHourlyDealsAsync();

                return Json(new
                {
                    success = true,
                    message = "Süresi geçen fırsatlar başarıyla temizlendi."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süresi geçen fırsatlar temizlenirken hata oluştu");
                return Json(new
                {
                    success = false,
                    message = "Süresi geçen fırsatlar temizlenirken bir hata oluştu."
                });
            }
        }
    }
}