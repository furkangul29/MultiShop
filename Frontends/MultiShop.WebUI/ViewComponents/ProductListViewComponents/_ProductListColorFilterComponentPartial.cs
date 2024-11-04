using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MultiShop.Catalog.Entites;
using MultiShop.WebUI.Models;
using System.Collections.Generic;

namespace MultiShop.WebUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListColorFilterComponentPartial : ViewComponent
    {
        private readonly IMongoCollection<Product> _productCollection;

        public _ProductListColorFilterComponentPartial(IMongoCollection<Product> productCollection)
        {
            _productCollection = productCollection;
        }

        public async Task< IViewComponentResult> InvokeAsync()
        {
            // MongoDB'den benzersiz renkleri çekiyoruz
            var distinctColors = await _productCollection.DistinctAsync<string>("Color", Builders<Product>.Filter.Empty);
            var colorsFromDb = distinctColors.ToList();
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
