using System.Net.Http.Json;
using MultiShop.DtoLayer.ImageDtos;

namespace MultiShop.WebUI.Services.ImageServices
{
    public class ImageService : IImageService
    {
        private readonly HttpClient _httpClient;

        public ImageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Dosya yükleme
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

            try
            {
                var response = await _httpClient.PostAsync("images/upload", content);

                // Log detailed error information
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine($"Error Content: {errorContent}");
                }

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<UploadImageResultDto>();
                return result?.Url ?? string.Empty;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return string.Empty;
            }
        }

        // Dosya silme
        public async Task<bool> DeleteImageAsync(string fileName)
        {
            var response = await _httpClient.DeleteAsync($"images/delete/{fileName}");
            return response.IsSuccessStatusCode;
        }

        // İmzalı URL alma
        public async Task<string> GetSignedUrlAsync(string fileName, int timeoutInMinutes = 30)
        {
            var response = await _httpClient.GetAsync($"images/signed-url/{fileName}?timeout={timeoutInMinutes}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SignedUrlResultDto>();
                return result?.Url ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
