// Controller
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Services.FavoriteProductServices;

namespace MultiShop.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteProductsController : ControllerBase
    {
        private readonly IFavoriteProductService _favoriteService;

        public FavoriteProductsController(IFavoriteProductService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<FavoriteProductDto>>> GetUserFavorites(string userId)
        {
            var favorites = await _favoriteService.GetUserFavorites(userId);
            return Ok(favorites);
        }

        [HttpGet("count/{userId}")]
        public async Task<ActionResult<int>> GetFavoriteCount(string userId)
        {
            var count = await _favoriteService.GetFavoriteCount(userId);
            return Ok(count);
        }

        [HttpGet("check/{productId}/{userId}")]
        public async Task<ActionResult<bool>> CheckIsFavorited(string productId, string userId)
        {
            var isFavorited = await _favoriteService.IsProductFavorited(productId, userId);
            return Ok(isFavorited);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> AddToFavorites([FromBody] FavoriteToggleRequest request)
        {
            var result = await _favoriteService.AddToFavorites(request.ProductId, request.UserId);
            return Ok(result);
        }

        [HttpDelete("{productId}/{userId}")]
        public async Task<ActionResult<bool>> RemoveFromFavorites(string productId, string userId)
        {
            var result = await _favoriteService.RemoveFromFavorites(productId, userId);
            return Ok(result);
        }

        [HttpPost("toggle")]
        public async Task<ActionResult<FavoriteToggleResponse>> ToggleFavorite(
            [FromBody] FavoriteToggleRequest request)
        {
            var isFavorited = await _favoriteService.IsProductFavorited(
                request.ProductId, request.UserId);

            if (isFavorited)
                await _favoriteService.RemoveFromFavorites(request.ProductId, request.UserId);
            else
                await _favoriteService.AddToFavorites(request.ProductId, request.UserId);

            var newCount = await _favoriteService.GetFavoriteCount(request.UserId);
            return Ok(new FavoriteToggleResponse
            {
                Success = true,
                Count = newCount,
                IsFavorited = !isFavorited
            });
        }
    }
}