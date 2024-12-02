using Microsoft.AspNetCore.Http;

namespace MultiShop.WebUI.Services.ImageServices
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string fileName);
        Task<string> GetSignedUrlAsync(string fileName, int timeoutInMinutes = 30);
    }
}
