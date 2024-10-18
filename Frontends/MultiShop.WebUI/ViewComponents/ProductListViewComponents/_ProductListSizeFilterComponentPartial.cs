using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Models;
using System.Collections.Generic;

namespace MultiShop.WebUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListSizeFilterComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var sizes = new List<FilterOption>
            {
                new FilterOption { Id = "size-all", Label = "Tüm Bedenler", Value = "all" },
                new FilterOption { Id = "size-1", Label = "XS", Value = "XS" },
                new FilterOption { Id = "size-2", Label = "S", Value = "S" },
                new FilterOption { Id = "size-3", Label = "M", Value = "M" },
                new FilterOption { Id = "size-4", Label = "L", Value = "L" },
                new FilterOption { Id = "size-5", Label = "XL", Value = "XL" }
            };

            return View(sizes);
        }
    }
}
