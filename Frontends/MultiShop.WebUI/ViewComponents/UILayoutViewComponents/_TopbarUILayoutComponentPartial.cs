using Microsoft.AspNetCore.Mvc;
using MultiShop.RapidApiWebUI.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiShop.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _TopbarUILayoutComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _rapidApiKey;

        public _TopbarUILayoutComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _rapidApiKey = configuration["RapidApiKey"];
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string cityName = await GetCityFromIpAsync(ipAddress);

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                var weatherData = await GetWeatherData(cityName);
                ViewBag.CityName = cityName;
                ViewBag.Temperature = weatherData?.current?.temp_c;
            }
            else
            {
                ViewBag.ErrorMessage = "Konum bilgisi alınamadı.";
            }

            return View();
        }

        private async Task<string> GetCityFromIpAsync(string ipAddress)
        {
            if (ipAddress == "127.0.0.1" || ipAddress == "::1")
            {
                return "Istanbul"; // Örnek şehir adı
            }
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://ip-api.com/json/{ipAddress}")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var locationData = JsonConvert.DeserializeObject<dynamic>(body);
                return locationData?.city;
            }
        }

        private async Task<WeatherViewModel.Rootobject> GetWeatherData(string cityName)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://weather-api-by-any-city.p.rapidapi.com/weather/{cityName}"),
                Headers =
                {
                    { "x-rapidapi-key", _rapidApiKey },
                    { "x-rapidapi-host", "weather-api-by-any-city.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherViewModel.Rootobject>(body);
            }
        }
    }
}
