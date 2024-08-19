using MultiShop.DtoLayer.CatalogDtos.CategoryDtos;

namespace MultiShop.WebUI.Models
{
    public class CombinedViewModel
    {
        public List<ResultCategoryDto> Categories { get; set; }
        public NavbarViewModel Navbar { get; set; }
    }
}
