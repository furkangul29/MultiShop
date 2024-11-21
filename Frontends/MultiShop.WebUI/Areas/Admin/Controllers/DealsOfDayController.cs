using AutoMapper;
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
        private readonly IMapper _mapper;


        public DealsOfDayController(IDealsOfDayService dealsOfDayService, ICategoryService categoryService, IProductService productService, IMapper mapper)
        {
            _dealsOfDayService = dealsOfDayService;
            _categoryService = categoryService;
            _productService = productService;
            _mapper = mapper;
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
        public async Task<IActionResult> CreateDealsOfDay([FromBody] CreateDealsOfDayDto createDealDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .First(); // İlk hatayı al
                    return BadRequest(new
                    {
                        success = false,
                        message = errors
                    });
                }

                // Kampanya oluşturma işlemi
                await _dealsOfDayService.CreateDealAsync(createDealDto);

                // Başarılı işlem durumu
                return Ok(new
                {
                    success = true,
                    message = "Günün fırsatı başarıyla oluşturuldu!"
                });
            }
            catch (Exception ex)
            {
                // Özel hata mesajlarını yakala
                if (ex.Message.Contains("aktif bir kampanya bulunmaktadır"))
                {
                    // Eğer aktif kampanya varsa, bu durumu toast bildirimi olarak dön
                    return BadRequest(new
                    {
                        success = false,
                        message = ex.Message // Bu mesaj Toastify'de gösterilecek
                    });
                }

                // Diğer tüm hatalar için genel hata durumu
                return StatusCode(500, new
                {
                    success = false,
                    message = "Fırsat oluşturulurken bir hata oluştu!" // Genel hata mesajı
                });
            }
        }

        [HttpGet]
    [Route("GetProductsByCategory/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(string categoryId)
    {
        try
        {
            var products = await _productService.GetProductsWithCategoryByCatetegoryIdAsync(categoryId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ürünler yüklenirken bir hata oluştu" });
        }
    }
        [HttpDelete]
        [Route("DeleteDealsOfDay/{id}")]
        
        public async Task<IActionResult> DeleteDealsOfDay(string id)
        {
            var success = await _dealsOfDayService.DeleteDealsOfDayAsync(id);
            if (success)
            {
                return Json(new { success = true }); // Başarı durumunda JSON döndürüyoruz
            }
            else
            {
                return Json(new { success = false }); // Başarısız durumda JSON döndürüyoruz
            }
        }


        [HttpGet]
        [Route("UpdateDealsOfDay/{id}")]
        public async Task<IActionResult> UpdateDealsOfDay(string id)
        {
            try
            {
                DealsOfDayViewbagList();

                // İlgili fırsat bilgisini getir
                var updateDealDto = await _dealsOfDayService.UpdateGetByIdDealAsync(id);

                // Görünümde kullan
                return View(updateDealDto);
            }
            catch (Exception ex)
            {
                // Hata yönetimi
                TempData["Error"] = "Fırsat bilgisi alınırken bir hata oluştu.";
                return RedirectToAction("Index", "DealsOfDay", new { area = "Admin" });
            }
        }


        [HttpPut]
        [Route("UpdateDealsOfDay/{id}")]
        public async Task<IActionResult> UpdateDealsOfDay(UpdateDealsOfDayDto updateDealDto)
        {
            try
            {
                var result =  _dealsOfDayService.UpdateDealAsync(updateDealDto);
                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(string id, bool status)
        {
            try
            {
                await _dealsOfDayService.ChangeDealStatusAsync(id, status);
                return Json(new
                {
                    success = true,
                    message = status ? "Ürün başarıyla aktife alındı ve indirimli fiyat uygulandı."
                                   : "Ürün başarıyla pasife alındı ve normal fiyata döndürüldü."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
