using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Models;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using System.Threading.Tasks;

namespace MultiShop.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _NavbarUILayoutComponentPartial : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        public _NavbarUILayoutComponentPartial(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllCategoryAsync();
            var navbarViewModel = new NavbarViewModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name
            };

            var combinedViewModel = new CombinedViewModel
            {
                Categories = categories,
                Navbar = navbarViewModel
            };

            return View(combinedViewModel);
        }
    }
}
