using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Services.DiscountServices;

namespace MultiShop.WebUI.Controllers
{
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;
        private readonly IBasketService _basketService;
        public DiscountController(IDiscountService discountService, IBasketService basketService)
        {
            _discountService = discountService;
            _basketService = basketService;
        }

        [HttpGet]
        public PartialViewResult ConfirmDiscountCoupon()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDiscountCoupon(string code)
        {
            try
            {
                // Fetch discount rate based on the coupon code
                var values = await _discountService.GetDiscountCouponCountRate(code);

                if (values == null || values <= 0)
                {
                    return Json(new { success = false, message = "Geçersiz kupon kodu!" });
                }

                var basketValues = await _basketService.GetBasket();
                if (basketValues == null || basketValues.TotalPrice <= 0)
                {
                    return Json(new { success = false, message = "Sepet bilgileri alınamadı!" });
                }

                decimal taxRate = 0.10m; // decimal literal for precision
                decimal totalPriceWithTax = basketValues.TotalPrice + (basketValues.TotalPrice * taxRate);
                decimal discountRate = values / 100m; // Ensure values are decimal
                decimal totalNewPriceWithDiscount = totalPriceWithTax - (totalPriceWithTax * discountRate);

                return Json(new
                {
                    success = true,
                    discountRate = values,
                    totalNewPriceWithDiscount = totalNewPriceWithDiscount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error applying discount coupon:", ex);
                return Json(new { success = false, message = "Kupon uygulanırken bir hata oluştu!" });
            }
        }
    }
}
