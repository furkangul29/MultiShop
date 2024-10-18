using Microsoft.AspNetCore.Mvc;
using MultiShop.DtoLayer.CatalogDtos.ProductDtos;
using MultiShop.DtoLayer.CommentDtos;
using MultiShop.WebUI.Models;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using Newtonsoft.Json;
using System.Text;

namespace MultiShop.WebUI.Controllers
{
    public class ProductListController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductService _productService;
        public ProductListController(IHttpClientFactory httpClientFactory, IProductService productService)
        {
            _httpClientFactory = httpClientFactory;
            _productService = productService;
        }
        public IActionResult Index(string id)
        {
            ViewBag.directory1 = "Ana Sayfa";
            ViewBag.directory2 = "Ürünler";
            ViewBag.directory3 = "Ürün Listesi";
            ViewBag.CategoryId = id;
            return View();
        }
  
        [HttpPost]
        public async Task<IActionResult> ApplyFilters([FromBody] FilterViewModel filters)
        {
            if (filters == null)
            {
                return BadRequest("Filtreler boş olamaz.");
            }

            try
            {
                // FilterViewModel'dan ProductFilterDto'ya dönüştür
                var productFilterDto = new ProductFilterDto
                {
                    CategoryId = filters.CategoryId,
                    SelectedPrices = filters.SelectedPrices?.Select(p => new PriceRangeDto
                    {
                        MinPrice = p.MinPrice,
                        MaxPrice = p.MaxPrice
                    }).ToList(),
                    SelectedColors = filters.SelectedColors,
                    SelectedSizes = filters.SelectedSizes
                };

                // API'den filtrelenmiş ürünleri al
                var filteredProducts = await _productService.GetFilteredProductsAsync(productFilterDto);

                // Ürünler null veya boşsa, mesajı ViewBag'e ekle
                if (filteredProducts == null || !filteredProducts.Any())
                {
                    ViewBag.Message = "Aradığınız kriterlere uygun ürün bulunamadı.";
                    ViewBag.FilteredProducts = null;
                }
                else
                {
                    ViewBag.FilteredProducts = filteredProducts;
                }

                // Index görünümüne filteredProducts ile dön
                Console.WriteLine("Filtered Products: " + JsonConvert.SerializeObject(filteredProducts));
                return View("Index", filteredProducts);
            }
            catch (HttpRequestException)
            {
                ViewBag.Message = "Sunucuya bağlanırken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View("Index");
            }
            catch (Exception)
            {
                ViewBag.Message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View("Index");
            }
        }



        public IActionResult ProductDetail(string id)
        {
            ViewBag.directory1 = "Ana Sayfa";
            ViewBag.directory2 = "Ürün Listesi";
            ViewBag.directory3 = "Ürün Detayları";
            ViewBag.ProductId = id;
            return View();
        }

        [HttpGet]
        public PartialViewResult AddComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CreateCommentDto createCommentDto)
        {
            // Formdan gelen RatingValue ve ProductId alınıyor
            var ratingValue = Request.Form["RatingValue"];
            var productId = Request.Form["ProductId"];

            // DTO'ya değerler atanıyor
            createCommentDto.Rating = int.Parse(ratingValue);
            createCommentDto.ProductId = productId;

            // Diğer özellikler
            createCommentDto.ImageUrl = "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?t=st=1721638657~exp=1721642257~hmac=dc8b679c1f68bcff3b3f8c124ee3af7ffad6a961daca46be40b2f8ba65689597&w=740";
            createCommentDto.CreatedDate = DateTime.Now;
            createCommentDto.Status = false;

            // HTTP istemcisi oluşturuluyor
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createCommentDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // API'ye POST isteği gönderiliyor
            var responseMessage = await client.PostAsync("https://localhost:7005/api/Comments", stringContent);

            // Yanıt başarılıysa yönlendirme yapılıyor
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Default");
            }

            // Hata durumunda aynı sayfayı döndürüyoruz
            return View();
        }


    }
}
