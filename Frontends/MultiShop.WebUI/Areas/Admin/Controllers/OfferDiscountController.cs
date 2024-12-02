using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiShop.DtoLayer.CatalogDtos.OfferDiscountDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using MultiShop.WebUI.Services.CatalogServices.OfferDiscountServices;
using MultiShop.WebUI.Services.ImageServices; // IImageService namespace'ini ekleyin
using Newtonsoft.Json;
using System.Text;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    [Route("Admin/OfferDiscount")]
    public class OfferDiscountController : Controller
    {
        private readonly IOfferDiscountService _offerDiscountService;
        private readonly ICategoryService _categoryService;
        private readonly IImageService _imageService; // ICloudStorageService yerine IImageService
        public OfferDiscountController(IOfferDiscountService offerDiscountService, ICategoryService categoryService, IImageService imageService)
        {
            _offerDiscountService = offerDiscountService;
            _categoryService = categoryService;
            _imageService = imageService; // DI'da değişiklik
        }

        void OfferDiscountViewBagList()
        {
            ViewBag.v1 = "Ana Sayfa";
            ViewBag.v2 = "İndirim Teklifleri";
            ViewBag.v3 = "İndirim Teklif Listesi";
            ViewBag.v0 = "İndirim Teklif İşlemleri";
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
            OfferDiscountViewBagList();
            var values = await _offerDiscountService.GetAllOfferDiscountAsync();
            return View(values);
        }

        [HttpGet]
        [Route("CreateOfferDiscount")]
        public async Task<IActionResult> CreateOfferDiscount()
        {
            await LoadCategoriesAsync();
            OfferDiscountViewBagList();
            return View();
        }

        [HttpPost]
        [Route("CreateOfferDiscount")]
        public async Task<IActionResult> CreateOfferDiscount(CreateOfferDiscountDto createOfferDiscountDto, IFormFile? ImageFile)
        {
            try
            {
                if (ImageFile != null)
                {
                    string uploadedUrl = await _imageService.UploadImageAsync(ImageFile);
                    createOfferDiscountDto.ImageUrl = uploadedUrl;
                }

                await _offerDiscountService.CreateOfferDiscountAsync(createOfferDiscountDto);
                return Json(new { success = true, message = "İndirim teklifi başarıyla Oluşturuldu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "İşlem sırasında bir hata oluştu." });
            }
        }

        [Route("DeleteOfferDiscount/{id}")]
        public async Task<IActionResult> DeleteOfferDiscount(string id)
        {
            await _offerDiscountService.DeleteOfferDiscountAsync(id);
            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }

        [Route("UpdateOfferDiscount/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateOfferDiscount(string id)
        {
            await LoadCategoriesAsync();
            OfferDiscountViewBagList();
            var values = await _offerDiscountService.GetByIdOfferDiscountAsync(id);
            return View(values);
        }

        [HttpPost]
        [Route("UpdateOfferDiscount/{id}")]
        public async Task<IActionResult> UpdateOfferDiscount([FromRoute] string id, [FromForm] UpdateOfferDiscountDto updateOfferDiscountDto, IFormFile? ImageFile)
        {
            try
            {
                // Model doğrulama
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, errors });
                }

                // İmaj yükleme işlemi
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadedUrl = await _imageService.UploadImageAsync(ImageFile);
                    updateOfferDiscountDto.ImageUrl = uploadedUrl;
                }

                // Güncelleme işlemi
                await _offerDiscountService.UpdateOfferDiscountAsync(updateOfferDiscountDto);

                return Ok(new
                {
                    success = true,
                    message = "İndirim teklifi başarıyla güncellendi."
                });
            }
            catch (Exception ex)
            {
                // Loglama (opsiyonel, ILogger veya özel log servisi kullanabilirsiniz)
                //_logger.LogError(ex, "Offer Discount Update Error");

                return BadRequest(new
                {
                    success = false,
                    message = "Bir hata oluştu. Lütfen tekrar deneyin."
                });
            }
        }

        private string GenerateFileNameToSave(string incomingFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{fileName}-{DateTime.Now.ToUniversalTime():yyyyMMddHHmmss}{extension}";
        }
    }
}
