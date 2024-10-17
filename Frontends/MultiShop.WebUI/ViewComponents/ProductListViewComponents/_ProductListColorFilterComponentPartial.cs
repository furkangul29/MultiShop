using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Models;

namespace MultiShop.WebUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListColorFilterComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var colors = new List<FilterOption>
            {
                new FilterOption { Id = "color-all", Label = "Tüm Renkler", Value = "all" },
                new FilterOption { Id = "color-1", Label = "Siyah", Value = "black" },
                new FilterOption { Id = "color-2", Label = "Beyaz", Value = "white" },
                new FilterOption { Id = "color-3", Label = "Kırmızı", Value = "red" },
                new FilterOption { Id = "color-4", Label = "Mavi", Value = "blue" },
                new FilterOption { Id = "color-5", Label = "Yeşil", Value = "green" }
            };
            return View(colors);
        }
    }
}
