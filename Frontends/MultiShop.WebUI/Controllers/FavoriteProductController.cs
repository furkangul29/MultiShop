namespace MultiShop.WebUI.Controllers
{
    using global::MultiShop.DtoLayer.CatalogDtos.ProductDtos;
    using global::MultiShop.WebUI.Services.CatalogServices.FavoriteProductServices;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace MultiShop.WebUI.Controllers
    {
        [Authorize]
        public class FavoriteProductController : Controller
            {
                private readonly IFavoriteProductService _favoriteService;

                public FavoriteProductController(IFavoriteProductService favoriteService)
                {
                    _favoriteService = favoriteService;
                }

           
                public async Task<IActionResult> Index()
                {
                    try
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var favorites = await _favoriteService.GetUserFavorites(userId);

                        if (favorites == null || !favorites.Any())
                        {
                            TempData["Error"] = "Favori ürünler bulunamadı.";
                        }

                        return View(favorites);
                    }
                    catch (Exception)
                    {
                        TempData["Error"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                        return View(new List<FavoriteProductDto>());
                    }
                }

                [HttpPost]
                public async Task<IActionResult> AddToFavorites(string productId)
                {
                    try
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var result = await _favoriteService.AddToFavorites(productId, userId);

                        if (!result)
                        {
                            return Json(new { success = false, message = "Ürün favorilere eklenirken bir hata oluştu." });
                        }

                        var newCount = await _favoriteService.GetFavoriteCount(userId);
                        return Json(new { success = true, message = "Ürün favorilere eklendi.", count = newCount });
                    }
                    catch (Exception)
                    {
                        return Json(new { success = false, message = "Bir hata oluştu." });
                    }
                }


            [HttpPost]
            public async Task<IActionResult> RemoveFromFavorites(string productId)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var result = await _favoriteService.RemoveFromFavorites(productId, userId);

                    if (!result)
                    {
                        return Json(new { success = false, message = "Ürün favorilerden kaldırılırken bir hata oluştu." });
                    }

                    var newCount = await _favoriteService.GetFavoriteCount(userId);
                    return Json(new { success = true, message = "Ürün favorilerden kaldırıldı.", count = newCount });
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Bir hata oluştu." });
                }
            }



            [HttpGet]
                public async Task<IActionResult> IsProductFavorited(string productId)
                {
                    try
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var isFavorited = await _favoriteService.IsProductFavorited(productId, userId);
                        return Json(new { success = true, isFavorited = isFavorited });
                    }
                    catch (Exception)
                    {
                        return Json(new { success = false, isFavorited = false });
                    }
                }

                [HttpGet]
                public async Task<IActionResult> GetFavoriteCount()
                {
                    try
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var count = await _favoriteService.GetFavoriteCount(userId);
                        return Json(new { success = true, count = count });
                    }
                    catch (Exception)
                    {
                        return Json(new { success = false, count = 0 });
                    }
                }
            }
        }
    }
