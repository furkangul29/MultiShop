using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Comment.Context;
using MultiShop.Comment.Entities;

namespace MultiShop.Comment.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentContext _context;
        public CommentsController(CommentContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CommentList()
        {
            var values = _context.UserComments.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateComment(UserComment userComment)
        {
            _context.UserComments.Add(userComment);
            _context.SaveChanges();
            return Ok("Yorum başarıyla eklendi");
        }

        [HttpDelete]
        public IActionResult DeleteComment(int id)
        {
            var value = _context.UserComments.Find(id);
            _context.UserComments.Remove(value);
            _context.SaveChanges();
            return Ok("Yorum başarıyla silindi");
        }

        [HttpGet("{id}")]
        public IActionResult GetComment(int id)
        {
            var value = _context.UserComments.Find(id);
            return Ok(value);
        }

        [HttpPut]
        public IActionResult UpdateComment(UserComment userComment)
        {
            _context.UserComments.Update(userComment);
            _context.SaveChanges();
            return Ok("Yorum başarıyla güncellendi");
        }

        [HttpGet("CommetListByProductId")]
        public IActionResult CommetListByProductId(string id)
        {
            var value = _context.UserComments.Where(x => x.ProductId == id).ToList();
            return Ok(value);
        }
        [HttpGet("GetActiveCommentCount")]
        public IActionResult GetActiveCommentCount()
        {
            var value = _context.UserComments.Where(x => x.Status == true).Count();
            return Ok(value);
        }
        [HttpGet("GetPassiveCommentCount")]
        public IActionResult GetPassiveCommentCount()
        {
            var value = _context.UserComments.Where(x => x.Status == false).Count();
            return Ok(value);
        }
        [HttpGet("GetTotalCommentCount")]
        public IActionResult GetTotalCommentCount()
        {
            var value = _context.UserComments.Count();
            return Ok(value);
        }


        [HttpGet("GetProductRatingStats")]
        public IActionResult GetProductRatingStats(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest("ProductId gereklidir.");
            }

            // Ürüne ait yorumları getir
            var comments = _context.UserComments
                .Where(x => x.ProductId == productId)
                .ToList();

            if (!comments.Any())
            {
                return Ok(new
                {
                    AverageRating = 0,
                    TotalReviews = 0
                });
            }

            var totalReviews = comments.Count(); // Toplam yorum sayısı
            var averageRating = Math.Round(comments.Average(x => x.Rating), 1); // Ortalama puan

            return Ok(new
            {
                AverageRating = averageRating,
                TotalReviews = totalReviews
            });
        }



    }
}
