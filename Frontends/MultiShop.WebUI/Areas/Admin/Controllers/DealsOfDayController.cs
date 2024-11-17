using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.DealsOfDayDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/DealsOfDay")]
    public class DealsOfDayController : Controller
    {
        private readonly IDealsOfDayService _dealsOfDayService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public DealsOfDayController(IDealsOfDayService dealsOfDayService, ICategoryService categoryService, IProductService productService)
        {
            _dealsOfDayService = dealsOfDayService;
            _categoryService = categoryService;
            _productService = productService;
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
        public async Task<IActionResult> CreateDealsOfDay()
        {
            DealsOfDayViewbagList();
            ViewBag.Categories = await _categoryService.GetAllCategoryAsync();
            return View();
        }

        [HttpPost]
        [Route("CreateDealsOfDay")]
        public async Task<IActionResult> CreateDealsOfDay(CreateDealsOfDayDto createDealDto)
        {
            createDealDto.IsActive = true;
            await _dealsOfDayService.CreateDealAsync(createDealDto);
            return RedirectToAction("Index", "DealsOfDay", new { area = "Admin" });
        }

        [HttpGet]
        [Route("GetProductsByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(string categoryId)
        {
            var products = await _productService.GetProductsWithCategoryByCatetegoryIdAsync(categoryId);
            return Json(products);
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
