using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiShop.DtoLayer.CatalogDtos.FeatureSliderDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/FeatureSlider")]
    public class FeatureSliderController : Controller
    {
        private readonly IFeatureSliderService _featureSliderService;
        private readonly ICategoryService _categoryService;

        public FeatureSliderController(IFeatureSliderService featureSliderService, ICategoryService categoryService)
        {
            _featureSliderService = featureSliderService;
            _categoryService = categoryService;
        }

        private void FeatureSliderViewBaglist()
        {
            ViewBag.v1 = "Ana Sayfa";
            ViewBag.v2 = "Öne Çıkan Görseller";
            ViewBag.v3 = "Slider Öne Çıkan Görsel Listesi";
            ViewBag.v0 = "Öne Çıkan Slider Görsel İşlemleri";
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoryAsync();
            ViewBag.Categories = categories
                .Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CategoryID
                })
                .ToList();
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            await LoadCategoriesAsync();
            var categories = await _categoryService.GetAllCategoryAsync();
            ViewBag.CategoryDictionary = categories.ToDictionary(
                x => x.CategoryID,
                x => x.CategoryName
            );
            FeatureSliderViewBaglist();
            var values = await _featureSliderService.GetAllFeatureSliderAsync();
            return View(values);
        }

        [HttpGet]
        [Route("CreateFeatureSlider")]
        public async Task<IActionResult> CreateFeatureSlider()
        {
            FeatureSliderViewBaglist();
            await LoadCategoriesAsync();
            return View();
        }

        [HttpPost]
        [Route("CreateFeatureSlider")]
        public async Task<IActionResult> CreateFeatureSlider(CreateFeatureSliderDto createFeatureSliderDto)
        {
            if (ModelState.IsValid)
            {
                await _featureSliderService.CreateFeatureSliderAsync(createFeatureSliderDto);
                return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
            }

            FeatureSliderViewBaglist();
            await LoadCategoriesAsync();
            return View(createFeatureSliderDto);
        }

        [Route("DeleteFeatureSlider/{id}")]
        public async Task<IActionResult> DeleteFeatureSlider(string id)
        {
            await _featureSliderService.DeleteFeatureSliderAsync(id);
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }

        [Route("UpdateFeatureSlider/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateFeatureSlider(string id)
        {
            FeatureSliderViewBaglist();
            await LoadCategoriesAsync();
            var values = await _featureSliderService.GetByIdFeatureSliderAsync(id);
            return View(values);
        }

        [Route("UpdateFeatureSlider/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateFeatureSlider(UpdateFeatureSliderDto updateFeatureSliderDto)
        {
            if (ModelState.IsValid)
            {
                await _featureSliderService.UpdateFeatureSliderAsync(updateFeatureSliderDto);
                return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
            }

            FeatureSliderViewBaglist();
            await LoadCategoriesAsync();
            return View(updateFeatureSliderDto);
        }

        [Route("FeatureSliderChangeStatusToTrue/{id}")]
        public async Task<IActionResult> FeatureSliderChangeStatusToTrue(string id)
        {
            await _featureSliderService.FeatureSliderChangeStatusToTrue(id);
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }

        [Route("FeatureSliderChangeStatusToFalse/{id}")]
        public async Task<IActionResult> FeatureSliderChangeStatusToFalse(string id)
        {
            await _featureSliderService.FeatureSliderChangeStatusToFalse(id);
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }
    }
}