using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.DealsOfDayServices;
using MultiShop.WebUI.Services.CatalogServices.HourlyDealServices;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.DefaultViewComponents
{
    public class _FeatureProductsDefaultComponentPartial : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IDealsOfDayService _dealsOfDayService;
        private readonly IHourlyDealService _hourlyDealService;
        public _FeatureProductsDefaultComponentPartial(IProductService productService, IHourlyDealService hourlyDealService, IDealsOfDayService dealsOfDayService)
        {
            _productService = productService;
            _hourlyDealService = hourlyDealService;
            _dealsOfDayService = dealsOfDayService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dealsOfDay = await _dealsOfDayService.GetAllDealsOfDayAsync();
            var hourlyDeals = await _hourlyDealService.GetCurrentHourlyDealsAsync();
            ViewBag.DealsOfDay = dealsOfDay;
            ViewBag.HourlyDeals = hourlyDeals;
            var values = await _productService.GetAllProductAsync();
            return View(values);
        }
    }
}
