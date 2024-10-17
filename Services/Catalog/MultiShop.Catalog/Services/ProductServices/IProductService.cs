using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.DtoLayer.CatalogDtos.ProductDtos;

namespace MultiShop.Catalog.Services.ProductServices
{
    public interface IProductService
    {
        Task<List<ResultProductDto>> GettAllProductAsync();
        Task CreateProductAsync(CreateProductDto createProductDto);
        Task UpdateProductAsync(UpdateProductDto updateProductDto);
        Task DeleteProductAsync(string id);
        Task<GetByIdProductDto> GetByIdProductAsync(string id);
        Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryAsync();
        Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryByCatetegoryIdAsync(string CategoryId);
        Task<List<ResultProductsWithCategoryDto>> GetFilteredProductsAsync(ProductFilterDto filterDto);
    }
}
