using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.BasketDtos;
using MultiShop.WebUI.Services.BasketServices;

public class _OrderSummaryComponentPartial : ViewComponent
{
    private readonly IBasketService _basketService;

    public _OrderSummaryComponentPartial(IBasketService basketService)
    {
        _basketService = basketService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var basketTotal = await _basketService.GetBasket();
        var basketItems = basketTotal.BasketItems;

        var shippingCost = CalculateShippingCost(basketItems);

        ViewBag.ShippingCost = shippingCost;

        return View(basketItems);
    }

    private decimal CalculateShippingCost(List<BasketItemDto> items)
    {
        decimal subtotal = items.Sum(item => item.Price * item.Quantity);

        if (subtotal >= 500m)
        {
            return 0m; // 500 TL ve üzeri siparişlerde kargo bedava
        }
        else
        {
            return 80m; // 500 TL altındaki siparişlerde sabit 80 TL kargo ücreti
        }
    }
}