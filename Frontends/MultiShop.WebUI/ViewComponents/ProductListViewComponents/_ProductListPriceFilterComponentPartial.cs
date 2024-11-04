using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MultiShop.Catalog.Entites;
using MultiShop.WebUI.Models;
using System.Collections.Generic;

namespace MultiShop.WebUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListPriceFilterComponentPartial : ViewComponent
    {
        private readonly IMongoCollection<Product> _productCollection;

        public _ProductListPriceFilterComponentPartial(IMongoCollection<Product> productCollection)
        {
            _productCollection = productCollection;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var priceStats = await _productCollection.Aggregate()
                .Group(x => 1, g => new
                {
                    MinPrice = g.Min(x => x.ProductPrice),
                    MaxPrice = g.Max(x => x.ProductPrice)
                })
                .FirstOrDefaultAsync();
            var priceRanges = new List<FilterOption>
            {
                new FilterOption { Id = "price-all", Label = $"Tüm Fiyatlar ({priceStats.MinPrice:N0}₺ - {priceStats.MaxPrice:N0}₺)",
                    Value = $"{priceStats.MinPrice}-{priceStats.MaxPrice}" },
                new FilterOption { Id = "price-1", Label = "0₺ - 999₺", Value = "0-999" },
                new FilterOption { Id = "price-2", Label = "1000₺ - 1999₺", Value = "1000-1999" },
                new FilterOption { Id = "price-3", Label = "2000₺ - 2999₺", Value = "2000-2999" },
                new FilterOption { Id = "price-4", Label = "3000₺ - 3999₺", Value = "3000-3999" },
                new FilterOption { Id = "price-5", Label = "4000₺ - 4999₺", Value = "4000-4999" }
            };

            return View(priceRanges);
        }
    }
}
