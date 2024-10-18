using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

public class _ProductListComponentPartial : ViewComponent
{
    private readonly IProductService _productService;

    public _ProductListComponentPartial(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string id, List<ProductDto> filteredProducts = null)
    {
        List<ResultProductWithCategoryDto> values;

        if (filteredProducts != null)
        {
            // Filtrelenmiş ürünler varsa, onları döndür
            if (filteredProducts.Any())
            {
                values = filteredProducts.Select(p => new ResultProductWithCategoryDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    CategoryId = p.CategoryId
                    // Diğer gerekli dönüşümleri yap
                }).ToList();
            }
            else
            {
                // Filtrelenmiş ancak boş bir sonuç döndüyse, "NoProducts" görünümünü döndür
                return View("NoProducts", ViewBag.Message ?? "Aradığınız kriterlere uygun ürün bulunamadı.");
            }
        }
        else
        {
            // Başlangıç senaryosu: filtrelenmemiş tüm ürünleri getir
            values = await _productService.GetProductsWithCategoryByCatetegoryIdAsync(id);
        }

        return View(values);
    }
}