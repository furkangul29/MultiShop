using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.BasketDtos;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.DiscountServices;

namespace MultiShop.WebUI.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBasketService _basketService;

        // Controller constructor'ı
        public ShoppingCartController(IProductService productService, IBasketService basketService)
        {
            _productService = productService;
            _basketService = basketService;
        }

        // Sepet sayfası görüntüleme
        public async Task<IActionResult> Index(string code, int discountRate, decimal totalNewPriceWithDiscount)
        {
            ViewBag.code = code;
            ViewBag.discountRate = discountRate;
            ViewBag.totalNewPriceWithDiscount = totalNewPriceWithDiscount;
            ViewBag.directory1 = "Ana Sayfa";
            ViewBag.directory2 = "Ürünler";
            ViewBag.directory3 = "Sepetim";

            // Sepet verilerini al
            var values = await _basketService.GetBasket();
            ViewBag.total = values.TotalPrice;

            var totalPriceWithTax = values.TotalPrice + values.TotalPrice / 100 * 10;
            var tax = values.TotalPrice / 100 * 10;

            ViewBag.totalPriceWithTax = totalPriceWithTax;
            ViewBag.tax = tax;

            return View();
        }

        // Sepet öğesi ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> AddBasketItem(string id)
        {
            try
            {
                var values = await _productService.GetByIdProductAsync(id);

                if (values == null)
                {
                    return Json(new { success = false, message = "Ürünü sepete eklemek için giriş yapmanız gerekmektedir." });
                }

                var items = new BasketItemDto
                {
                    ProductId = values.ProductId,
                    ProductName = values.ProductName,
                    Price = values.ProductPrice,
                    Quantity = 1,
                    ProductImageUrl = values.ProductImageUrl
                };

                await _basketService.AddBasketItem(items);

                // Sepet öğe sayısını güncelle
                var itemCount = await _basketService.GetBasketItemCount();

                return Json(new { success = true, message = "Ürün sepete eklendi.", itemCount });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new { success = false, message = "Ürünü sepete eklemek için giriş yapmanız gerekmektedir." });
            }
            catch (ArgumentException ex) when (ex.ParamName == "refresh_token")
            {
                return Json(new { success = false, message = "Ürünü sepete eklemek için giriş yapmanız gerekmektedir." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        // Sepet öğesi çıkarma işlemi
        public async Task<IActionResult> RemoveBasketItem(string id)
        {
            await _basketService.RemoveBasketItem(id);

            // Sepet öğe sayısını güncelle
            var itemCount = await _basketService.GetBasketItemCount();

            return RedirectToAction("Index", new { itemCount });
        }

        // Sepet öğe sayısını döndürme
        public async Task<IActionResult> GetBasketItemCount()
        {
            var values = await _basketService.GetBasketItemCount();
            return Json(new { ItemCount = values });
        }
    }
}
