using MultiShop.DtoLayer.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Models;

namespace MultiShop.WebUI.Services.CatalogServices.ProductServices
{
    public interface IProductService
    {
        Task<List<ResultProductDto>> GetAllProductAsync();
        Task CreateProductAsync(CreateProductDto createProductDto);
        Task UpdateProductAsync(UpdateProductDto updateProductDto);
        Task DeleteProductAsync(string id);
        Task<UpdateProductDto> GetByIdProductAsync(string id);
        Task<List<ResultProductWithCategoryDto>> GetProductsWithCategoryAsync();
        Task<List<ResultProductWithCategoryDto>> GetProductsWithCategoryByCatetegoryIdAsync(string CategoryId);
        Task<List<ResultProductDto>> GetFilteredProductsAsync(ProductFilterDto filters);

    }
}
