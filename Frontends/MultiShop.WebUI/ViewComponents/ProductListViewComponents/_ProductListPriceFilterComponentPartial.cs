using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Models;
using System.Collections.Generic;

namespace MultiShop.WebUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListPriceFilterComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var priceRanges = new List<FilterOption>
            {
                new FilterOption { Id = "price-all", Label = "Tüm Fiyatlar", Value = "all" },
                new FilterOption { Id = "price-1", Label = "0₺ - 1000₺", Value = "0-1000" },
                new FilterOption { Id = "price-2", Label = "1000₺ - 2000₺", Value = "1000-2000" },
                new FilterOption { Id = "price-3", Label = "2000₺ - 3000₺", Value = "2000-3000" },
                new FilterOption { Id = "price-4", Label = "3000₺ - 4000₺", Value = "3000-4000" },
                new FilterOption { Id = "price-5", Label = "4000₺ - 5000₺", Value = "4000-5000" }
            };

            return View(priceRanges);
        }
    }
}