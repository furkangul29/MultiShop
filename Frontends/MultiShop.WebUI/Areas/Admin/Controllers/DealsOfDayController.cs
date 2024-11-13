using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;
using MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/DealsOfDay")]
    public class DealsOfDayController : Controller
    {
        private readonly IDealsOfDayService _dealsOfDayService;

        public DealsOfDayController(IDealsOfDayService dealsOfDayService)
        {
            _dealsOfDayService = dealsOfDayService;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            DealsOfDayViewbagList();
            var values = await _dealsOfDayService.GetAllDealsOfDayAsync();
            return View(values);
        }

        [HttpGet]
        [Route("CreateDealsOfDay")]
        public IActionResult CreateDealsOfDay()
        {
            DealsOfDayViewbagList();
            return View();
        }

        [HttpPost]
        [Route("CreateDealsOfDay")]
        public async Task<IActionResult> CreateDealsOfDay(CreateDealsOfDayDto createDealDto)
        {
            await _dealsOfDayService.CreateDealAsync(createDealDto);
            return RedirectToAction("Index", "DealsOfDay", new { area = "Admin" });
        }

        [Route("DeleteDealsOfDay/{id}")]
        public async Task<IActionResult> DeleteDealsOfDay(string id)
        {
            await _dealsOfDayService.DeleteDealAsync(id);
            return RedirectToAction("Index", "DealsOfDay", new { area = "Admin" });
        }

        [HttpGet]
        [Route("UpdateDealsOfDay/{id}")]
        public async Task<IActionResult> UpdateDealsOfDay(string id)
        {
            DealsOfDayViewbagList();
            var values = await _dealsOfDayService.GetByIdDealAsync(id);
            return View(values);
        }

        [HttpPost]
        [Route("UpdateDealsOfDay/{id}")]
        public async Task<IActionResult> UpdateDealsOfDay(UpdateDealsOfDayDto updateDealDto)
        {
            await _dealsOfDayService.UpdateDealAsync(updateDealDto);
            return RedirectToAction("Index", "DealsOfDay", new { area = "Admin" });
        }

        void DealsOfDayViewbagList()
        {
            ViewBag.v1 = "Ana Sayfa";
            ViewBag.v2 = "Günün Fırsatları";
            ViewBag.v3 = "Günün Fırsatları Listesi";
            ViewBag.v0 = "Günün Fırsatları İşlemleri";
        }
    }
}
