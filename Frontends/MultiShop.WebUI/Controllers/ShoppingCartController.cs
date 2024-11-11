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
        public ShoppingCartController(IProductService productService, IBasketService basketService)
        {
            _productService = productService;
            _basketService = basketService;
        }
        public async Task<IActionResult> Index(string code,int discountRate,decimal totalNewPriceWithDiscount)
        {
            ViewBag.code = code;
            ViewBag.discountRate = discountRate;
            ViewBag.totalNewPriceWithDiscount = totalNewPriceWithDiscount;
            ViewBag.directory1 = "Ana Sayfa";
            ViewBag.directory2 = "Ürünler";
            ViewBag.directory3 = "Sepetim";
            var values = await _basketService.GetBasket();
            ViewBag.total = values.TotalPrice;
            var totalPriceWithTax = values.TotalPrice + values.TotalPrice / 100 * 10;
            var tax = values.TotalPrice / 100 * 10;
            ViewBag.totalPriceWithTax = totalPriceWithTax;
            ViewBag.tax = tax;
            return View();
        }

        //[HttpPost]
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
                return Json(new { success = true, message = "Ürün sepete eklendi." });
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




        public async Task<IActionResult> RemoveBasketItem(string id)
        {
            await _basketService.RemoveBasketItem(id);
            return RedirectToAction("Index");
        }
    }
}
