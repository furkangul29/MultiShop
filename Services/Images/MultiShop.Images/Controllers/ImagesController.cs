using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Images.Services;

namespace MultiShop.Images.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;

        public ImagesController(ICloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Geçersiz dosya.");

            string fileName = GenerateFileNameToSave(file.FileName);
            string uploadedUrl = await _cloudStorageService.UploadFileAsync(file, fileName);

            return Ok(new { Url = uploadedUrl });
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            await _cloudStorageService.DeleteFileAsync(fileName);
            return NoContent();
        }

        [HttpGet("signed-url/{fileName}")]
        public async Task<IActionResult> GetSignedUrl(string fileName, [FromQuery] int timeout = 30)
        {
            string signedUrl = await _cloudStorageService.GetSignedUrlAsync(fileName, timeout);
            return Ok(new { Url = signedUrl });
        }

        private string GenerateFileNameToSave(string incomingFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{fileName}-{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        }
    }
}

